using AdjusterOptimizerAPI.Data;
using AdjusterOptimizerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace AdjusterOptimizerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ------------------------------------------------------------
        // LOGIN
        // ------------------------------------------------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Username and password are required." });
            }

            // Retrieve user by username
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            // Validate password hash
            bool passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!passwordValid)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            // Store session values
            HttpContext.Session.SetInt32("USER_ID", user.UserId);
            HttpContext.Session.SetString("ROLE", user.Role);
            HttpContext.Session.SetString("USERNAME", user.Username);

            return Ok(new
            {
                message = "Login successful",
                user = new
                {
                    user.UserId,
                    user.Username,
                    user.Role
                }
            });
        }

        // ------------------------------------------------------------
        // LOGOUT
        // ------------------------------------------------------------
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok(new { message = "Logged out successfully." });
        }
    }

    // ------------------------------------------------------------
    // LOGIN REQUEST MODEL
    // ------------------------------------------------------------
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
