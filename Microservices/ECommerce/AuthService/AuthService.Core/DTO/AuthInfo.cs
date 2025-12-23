namespace AuthService.Core.DTO
{
    public class AuthInfo
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string RoleName { get; set; }
    }
}
