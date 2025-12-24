using AuthService.Core.DTO;
using AuthService.Core.Models;

namespace AuthService.Application.Services.RefreshTokenService
{
    public interface IRefreshTokenService
    {
        Task<TokenResponse> RefreshAsync(string refreshToken, TokenPayload tokenPayload);
    }
}
