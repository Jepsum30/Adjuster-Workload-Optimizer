using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdjusterOptimizerAPI.Models
{
    /// <summary>
    /// Represents a system user who can register, authenticate,
    /// and access application features based on assigned role.
    /// </summary>
    [Table("users")]
    public class User
    {
        /// <summary>
        /// Primary key for the Users table.
        /// Maps to USER_ID in MySQL.
        /// </summary>
        [Column("USER_ID")]
        public int UserId { get; set; }

        /// <summary>
        /// Username used for authentication.
        /// Required for registration and login.
        /// </summary>
        [Required]
        [Column("USERNAME")]
        public required string Username { get; set; }

        /// <summary>
        /// Hashed password stored securely in the database.
        /// </summary>
        [Required]
        [Column("PASSWORD_HASH")]
        public required string PasswordHash { get; set; }

        /// <summary>
        /// User role (Admin, Adjuster, Supervisor, Manager, etc.).
        /// Determines access permissions.
        /// </summary>
        [Required]
        [Column("ROLE")]
        public required string Role { get; set; }

        /// <summary>
        /// Email address used for notifications and password recovery.
        /// </summary>
        [Required]
        [Column("EMAIL")]
        public required string Email { get; set; }

        /// <summary>
        /// Validates password complexity rules for registration
        /// and password changes.
        /// </summary>
        public static bool ValidatePassword(string password)
        {
            return password.Length >= 8 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsLower) &&
                   password.Any(char.IsDigit) &&
                   password.Any(ch => "!@#$%^&*".Contains(ch));
        }
    }
}
