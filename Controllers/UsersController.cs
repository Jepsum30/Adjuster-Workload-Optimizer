using AdjusterOptimizerAPI.Data;
using AdjusterOptimizerAPI.Models;
using AdjusterOptimizerAPI.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        // GET USER BY ID (Admin or self)
        // ------------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sessionUserId = HttpContext.Session.GetInt32("USER_ID");
            var sessionRole = HttpContext.Session.GetString("ROLE");

            if (sessionUserId == null)
                return Unauthorized("Not logged in.");

            if (sessionRole != "Admin" && sessionUserId != id)
                return Unauthorized("Access denied.");

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound("User not found.");

            return Ok(user);
        }

        // ------------------------------------------------------------
        // REGISTER NEW USER
        // ------------------------------------------------------------
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            if (!ValidatePassword(req.Password))
                return BadRequest("Password must include uppercase, lowercase, number, and symbol.");

            var existing = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == req.Username);

            if (existing != null)
                return BadRequest("Username or email already exists.");

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
        // LOGIN
        // ------------------------------------------------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == req.Username);

            if (user == null)
                return Unauthorized("Invalid username or password.");

            if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
                return Unauthorized("Invalid username or password.");

            // Store session
            HttpContext.Session.SetInt32("USER_ID", user.UserId);
            HttpContext.Session.SetString("ROLE", user.Role);

            return Ok(new
            {
                message = "Login successful.",
                role = user.Role,
                userId = user.UserId
            });
        }

        // ------------------------------------------------------------
        // LOGOUT
        // ------------------------------------------------------------
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok("Logged out.");
        }

        // ------------------------------------------------------------
        // CHANGE PASSWORD (self only)
        // ------------------------------------------------------------
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] string newPassword)
        {
            var userId = HttpContext.Session.GetInt32("USER_ID");
            if (userId == null)
                return Unauthorized("Not logged in.");

            var user = await _context.Users.FindAsync(userId);
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
        // VALIDATE PASSWORD
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
}
