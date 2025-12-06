using System.Security.Cryptography;

namespace Ecommerce.Application.Services.PasswordService
{
    public sealed class PasswordService : IPasswordService
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 100_000;

        public string HashPassword(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);

            var key = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                KeySize
            );

            // store: version(1) | iterations(4) | salt | key
            var result = new byte[1 + 4 + SaltSize + KeySize];
            result[0] = 1;
            BitConverter.GetBytes(Iterations).CopyTo(result, 1);
            Buffer.BlockCopy(salt, 0, result, 1 + 4, SaltSize);
            Buffer.BlockCopy(key, 0, result, 1 + 4 + SaltSize, KeySize);

            return Convert.ToBase64String(result);
        }

        public bool VerifyPassword(string hashedPasswordWithSalt, string password)
        {
            var data = Convert.FromBase64String(hashedPasswordWithSalt);
            if (data.Length < 1 + 4 + SaltSize + 1) return false;

            var version = data[0];
            var iterations = BitConverter.ToInt32(data, 1);
            var salt = new byte[SaltSize];
            var key = new byte[KeySize];
            Buffer.BlockCopy(data, 1 + 4, salt, 0, SaltSize);
            Buffer.BlockCopy(data, 1 + 4 + SaltSize, key, 0, KeySize);

            var computedKey = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                KeySize
            );

            return CryptographicOperations.FixedTimeEquals(key, computedKey);
        }
    }
}
