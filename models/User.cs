using System.Linq;                     // ⭐ REQUIRED for .Any()
using System.ComponentModel.DataAnnotations.Schema;

namespace AdjusterOptimizerAPI.Models
{
    [Table("users")]
    public class User
    {
        [Column("USER_ID")]
        public int UserId { get; set; }

        [Column("USERNAME")]
        public string Username { get; set; } = string.Empty;

        [Column("PASSWORD_HASH")]
        public string PasswordHash { get; set; } = string.Empty;

        [Column("ROLE")]
        public string Role { get; set; } = string.Empty;

        public static bool ValidatePassword(string password)
        {
            return password.Length >= 8 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsLower) &&
                   password.Any(char.IsDigit) &&
                   password.Any(ch => !char.IsLetterOrDigit(ch));
        }
    }
}
