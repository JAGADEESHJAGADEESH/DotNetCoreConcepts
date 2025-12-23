using AuthService.Core.DTO;
using AuthService.Core.Models;

namespace AuthService.Application.Services.TokenService
{
    public interface ITokenService
    {
        Task<TokenResponse> GenerateTokenAsync(TokenPayload tokenPayLoad);
    }

}
