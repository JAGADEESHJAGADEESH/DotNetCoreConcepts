namespace AuthService.Core.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public byte[] PasswordHash { get; set; } = null!;
        public byte[] PasswordSalt { get; set; } = null!;
        public Roles Role { get; set; } 
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
