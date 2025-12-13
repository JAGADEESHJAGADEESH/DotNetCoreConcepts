namespace Ecommerce.Core.Models
{
    public class RegisterResponse
    {
        public bool Success { get; set; }
        public Users User { get; set; }
        public Token Token { get; set; }
        public string? Message { get; set; }
    }

}
