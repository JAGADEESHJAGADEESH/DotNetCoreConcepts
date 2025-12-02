using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Ecommerce.Core.Models;
using ECommerce.Infrastructure.TokenRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Ecommerce.Application.Services.TokenService
{
    public class TokenService : ITokenService
    {
        private readonly ILogger<TokenService> _logger;
        private readonly ITokenRepository _tokenRepository;
        private readonly IConfiguration _configuration;

        public TokenService(ILogger<TokenService> logger, ITokenRepository tokenRepository, IConfiguration configuration)
        {
            _logger = logger;
            _tokenRepository = tokenRepository;
            _configuration = configuration;
        }

        public async Task<Token> GenerateTokenAsync(Users user, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Generating tokens for user {Username}", user.UserName);

            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"] ?? "ECommerceWebAPI";
            if (string.IsNullOrEmpty(key))
            {
                throw new InvalidOperationException("JWT signing key is not configured.");
            }

            var keyBytes = Encoding.UTF8.GetBytes(key);
            var securityKey = new SymmetricSecurityKey(keyBytes);
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // jti used for revocation/tracking
            var jti = Guid.NewGuid().ToString();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, jti),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
            };

            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(60); // short-lived
            var jwt = new JwtSecurityToken(
                issuer: issuer,
                audience: issuer,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: accessTokenExpiration,
                signingCredentials: signingCredentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

            // Create a secure refresh token (random 64 bytes, base64)
            var refreshToken = GenerateSecureRandomString(64);

            // Store refresh token metadata (hash) in DB via repository
            var refreshTokenHash = ComputeSha256Hash(refreshToken);
            var record = new Token
            {
                UserId = user.UserId,
                AccessToken = accessToken,
                RefreshToken = refreshTokenHash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                Revoked = false
            };

            try 
            {
                var isSuccess = await _tokenRepository.SaveTokenAsync(record, cancellationToken);
                if (isSuccess)
                {
                    return new Token
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging token storage for user {Username}", user.UserName);
            }

            return null!;
        }

        private static string GenerateSecureRandomString(int sizeInBytes)
        {
            var bytes = RandomNumberGenerator.GetBytes(sizeInBytes);
            return Convert.ToBase64String(bytes);
        }
            
        private static string ComputeSha256Hash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
