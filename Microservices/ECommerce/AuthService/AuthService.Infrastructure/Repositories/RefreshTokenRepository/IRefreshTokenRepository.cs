using AuthService.Core.Models;

namespace AuthService.Infrastructure.Repositories.RefreshTokenRepository
{
    public interface IRefreshTokenRepository
    {
        Task CreateAsync(RefreshToken token);
        Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);
        Task RevokeAsync(Guid tokenId);
    }

}
