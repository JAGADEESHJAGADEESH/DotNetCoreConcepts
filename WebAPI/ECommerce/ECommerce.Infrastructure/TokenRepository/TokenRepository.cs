using Ecommerce.Core.Models;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.TokenRepository
{
    public class TokenRepository : ITokenRepository
    {
        private readonly ILogger<TokenRepository> _logger;
        public TokenRepository(ILogger<TokenRepository> logger)
        {
            _logger = logger;
        }

        public async Task<bool> SaveTokenAsync(Token record, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Saving token for user {record.UserId}");
            // Simulate saving the token
            return false;
        }
    }
}
