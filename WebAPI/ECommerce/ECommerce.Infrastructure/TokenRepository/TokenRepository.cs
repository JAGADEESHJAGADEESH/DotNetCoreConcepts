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
        public Task SaveTokenAsync(string token, int userId)
        {
            _logger.LogInformation($"Saving token for user {userId}");
            throw new NotImplementedException();
        }

        public Task<bool> IsTokenValidAsync(string token, int userId)
        {
            _logger.LogInformation($"Checking if token is valid for user {userId}");
            throw new NotImplementedException();
        }

        public Task InvalidateTokenAsync(string token, int userId)
        {
            _logger.LogInformation($"Invalidating token for user {userId}");
            throw new NotImplementedException();
        }
    }
}
