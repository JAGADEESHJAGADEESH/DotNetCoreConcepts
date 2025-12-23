using AuthService.Core.DTO;
using AuthService.Core.Models;
using AuthService.Infrastructure.Repositories.RefreshTokenRepository;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthService.Application.Services.TokenService
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _settings;
        private readonly IRefreshTokenRepository _refreshTokenRepo;

        public TokenService(
            IOptions<JwtSettings> settings,
            IRefreshTokenRepository refreshTokenRepo)
        {
            _settings = settings.Value;
            _refreshTokenRepo = refreshTokenRepo;
        }

        public async Task<TokenResponse> GenerateTokenAsync(TokenPayload tokenPayLoad)
        {
            // ===== Claims =====
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, tokenPayLoad.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, tokenPayLoad.EmailAddress),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, tokenPayLoad.RoleName)
            };

            // ===== Signing =====
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_settings.Key));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // ===== Expiry =====
            var accessTokenExpiry = DateTime.UtcNow
                .AddMinutes(_settings.ExpiryMinutes);

            // ===== Access Token =====
            var jwtToken = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: accessTokenExpiry,
                signingCredentials: creds
            );

            var accessToken = new JwtSecurityTokenHandler()
                .WriteToken(jwtToken);

            // ===== Refresh Token =====
            var refreshToken = GenerateRefreshToken();
            var refreshTokenHash = HashToken(refreshToken);

            await _refreshTokenRepo.CreateAsync(new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = tokenPayLoad.Id,
                TokenHash = refreshTokenHash,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            });

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = accessTokenExpiry
            };
        }

        private static string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        private static string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(token);
            return Convert.ToBase64String(sha256.ComputeHash(bytes));
        }
    }
}
