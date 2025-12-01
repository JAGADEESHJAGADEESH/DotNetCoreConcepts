using Ecommerce.Core.Models;

namespace ECommerceWebAPI.Services.Token
{
    public interface ITokenService
    {
        Task<string> CreateToken(User user);
    }
}
