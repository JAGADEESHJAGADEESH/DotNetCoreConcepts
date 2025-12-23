namespace AuthService.Core.DTO
{
    public class TokenPayload
    {
        public Guid Id { get; set; }
        public string EmailAddress { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }
}
