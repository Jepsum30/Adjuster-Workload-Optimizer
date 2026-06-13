using AdjusterOptimizerAPI.Data;
using AdjusterOptimizerAPI.Models;
using AdjusterOptimizerAPI.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AdjusterOptimizerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ------------------------------------------------------------
        // GET ALL USERS (Admin only)
        // ------------------------------------------------------------
        [HttpGet]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        // ------------------------------------------------------------
        // GET USER BY ID (Admin OR self)
        // ------------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("Not logged in.");

            if (role != "Admin" && userId != id.ToString())
                return Unauthorized("Access denied.");

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound("User not found.");

            return Ok(user);
        }

        // ------------------------------------------------------------
        // REGISTER NEW USER (Admin only)
        // ------------------------------------------------------------
        [HttpPost("register")]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            if (!ValidatePassword(req.Password))
                return BadRequest("Password must include uppercase, lowercase, number, and symbol.");

            var existing = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == req.Username);

            if (existing != null)
                return BadRequest("Username already exists.");

            var user = new User
            {
                Username = req.Username,
                Role = req.Role,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Account created successfully.");
        }

        // ------------------------------------------------------------
        // CHANGE PASSWORD (self only)
        // ------------------------------------------------------------
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] string newPassword)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("Not logged in.");

            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user == null)
                return NotFound("User not found.");

            if (!ValidatePassword(newPassword))
                return BadRequest("Password must include uppercase, lowercase, number, and symbol.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _context.SaveChangesAsync();

            return Ok("Password updated successfully.");
        }

        // ------------------------------------------------------------
        // DELETE USER (Admin only)
        // ------------------------------------------------------------
        [HttpDelete("{id}")]
        [RoleAuthorize("Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound("User not found.");

            if (user.Role == "Admin")
                return BadRequest("Cannot delete another Admin.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok("User deleted successfully.");
        }

        // ------------------------------------------------------------
        // PASSWORD VALIDATION
        // ------------------------------------------------------------
        private bool ValidatePassword(string password)
        {
            return !string.IsNullOrEmpty(password) &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsLower) &&
                   password.Any(char.IsDigit) &&
                   password.Any(c => !char.IsLetterOrDigit(c));
        }
    }

    // ------------------------------------------------------------
    // REQUEST MODELS
    // ------------------------------------------------------------
    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = "Adjuster";
        public string Password { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
