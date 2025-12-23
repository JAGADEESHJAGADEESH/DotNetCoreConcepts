using AuthService.Application.Services.TokenService;
using AuthService.Core.DTO;
using AuthService.Core.Models;
using AuthService.Infrastructure.Repositories.RefreshTokenRepository;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace AuthService.Application.Services.RefreshTokenService
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ITokenService _tokenService;

        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository, ITokenService tokenService)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _tokenService = tokenService;
        }

        public async Task<TokenResponse> RefreshAsync(string refreshToken, TokenPayload tokenPayload)
        {
            var tokenHash = ComputeHash(refreshToken);

            var storedToken = await _refreshTokenRepository
                .GetByTokenHashAsync(tokenHash);

            if (storedToken is null || storedToken.IsRevoked)
                throw new UnauthorizedAccessException("Invalid refresh token");

            if (storedToken.ExpiresAt <= DateTime.UtcNow)
                throw new UnauthorizedAccessException("Refresh token expired");

            await _refreshTokenRepository.RevokeAsync(storedToken.Id);

            var newRefreshToken = GenerateRefreshToken();
            var newRefreshTokenHash = ComputeHash(newRefreshToken);

            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = tokenPayload.Id,
                TokenHash = newRefreshTokenHash,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            await _refreshTokenRepository.CreateAsync(refreshTokenEntity);

            return await _tokenService.GenerateTokenAsync(tokenPayload);
            
        }

        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        private static string ComputeHash(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(bytes);
        }
    }

}
