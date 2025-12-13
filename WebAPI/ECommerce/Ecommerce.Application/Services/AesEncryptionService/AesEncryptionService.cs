using System.Security.Cryptography;
using System.Text;

namespace Ecommerce.Application.Services.AesEncryptionService
{
    public class AesEncryptionService
    {
        private readonly byte[] _key; // 32 bytes for AES-256
        private readonly byte[] _iv;  // 16 bytes initialization vector

        public AesEncryptionService(string key, string iv)
        {
            // Ensure key and IV are the correct length
            _key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
            _iv = Encoding.UTF8.GetBytes(iv.PadRight(16).Substring(0, 16));
        }

        public string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }
            return Convert.ToBase64String(ms.ToArray());
        }

        public string Decrypt(string cipherText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
    }


}
