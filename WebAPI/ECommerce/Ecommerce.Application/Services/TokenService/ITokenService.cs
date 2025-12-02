using Ecommerce.Core.Models;

namespace Ecommerce.Application.Services.TokenService
{
    public interface ITokenService
    {
        Task<Token> GenerateTokenAsync(Users user, CancellationToken cancellationToken = default);
    }
}
