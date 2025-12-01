using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Core.Models
{

    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = null!;

        // Store hashed password
        [Required]
        public string PasswordHash { get; set; } = null!;

        // Optional role or claims
        public string? Role { get; set; }

    }
}
