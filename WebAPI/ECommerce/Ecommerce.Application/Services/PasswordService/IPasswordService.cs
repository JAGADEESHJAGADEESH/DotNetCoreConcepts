namespace Ecommerce.Application.Services.PasswordService
{
    public interface IPasswordService
    {
        string HashPassword(string password);
        bool VerifyPassword(string hashedPasswordWithSalt, string password);
    }
}
