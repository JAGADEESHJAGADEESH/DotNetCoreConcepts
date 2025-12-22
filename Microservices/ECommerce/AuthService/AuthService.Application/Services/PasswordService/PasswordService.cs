using System.Text;

namespace AuthService.Application.Services.PasswordService
{
    public class PasswordService : IPasswordService
    {
        public async Task<(byte[], byte[])> CreatePasswordHashAsync(string password)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            byte[] salt = hmac.Key;
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return (hash, salt);
        }

        public async Task<bool> VerifyPasswordAsync(string password, byte[] hash, byte[] salt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512(salt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(hash);
        }
    }
}