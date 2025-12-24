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

        public async Task<TokenResponse> GenerateTokenAsync(TokenPayload payload)
        {
            // ===== Claims =====
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, payload.Id.ToString()), // ✅ REQUIRED
                new Claim(JwtRegisteredClaimNames.Sub, payload.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, payload.EmailAddress),
                new Claim(ClaimTypes.Role, payload.RoleName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // ===== Signing =====
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_settings.Key));

            var credentials = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256);

            // ===== Expiry =====
            var accessTokenExpiresAt = DateTime.UtcNow
                .AddMinutes(_settings.ExpiryMinutes);

            // ===== Access Token =====
            var jwtToken = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: accessTokenExpiresAt,
                signingCredentials: credentials
            );

            var accessToken = new JwtSecurityTokenHandler()
                .WriteToken(jwtToken);

            // ===== Refresh Token =====
            var refreshToken = GenerateRefreshToken();
            var refreshTokenHash = HashToken(refreshToken);

            var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            await _refreshTokenRepo.CreateAsync(new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = payload.Id,
                TokenHash = refreshTokenHash,
                ExpiresAt = refreshTokenExpiry,
                IsRevoked = false
            });

            return new TokenResponse
            {
                AccessToken = accessToken,
                AccessTokenExpiresAt = accessTokenExpiresAt,
                RefreshToken = refreshToken,
                RefreshTokenExpiresAt = refreshTokenExpiry
            };
        }

        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(64));
        }

        private static string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            return Convert.ToBase64String(
                sha256.ComputeHash(Encoding.UTF8.GetBytes(token)));
        }
    }
}
