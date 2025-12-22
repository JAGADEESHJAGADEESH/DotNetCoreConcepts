namespace AuthService.Application.Services.PasswordService
{
    public interface IPasswordService
    {
        Task<(byte[], byte[])> CreatePasswordHashAsync(string password);
        Task<bool> VerifyPasswordAsync(string password, byte[] hash, byte[] salt);
    }
}
