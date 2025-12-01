using Ecommerce.Core.Models;
using Microsoft.Extensions.Logging;

namespace ECommerceWebAPI.Services.Token
{
    public class TokenService : ITokenService
    {
        private readonly ILogger<TokenService> _logger;
        public TokenService(ILogger<TokenService> logger)
        {
           _logger = logger;
        }
        public async Task<string> CreateToken(User user)
        {
            // Implementation for creating JWT token goes here
            _logger.LogInformation("Creating token for user {@User}", user.Username);

            throw new NotImplementedException();
        }
    }
}
