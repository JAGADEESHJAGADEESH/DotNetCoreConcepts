using AuthService.Core.Constants;
using AuthService.Core.Models;
using DatabaseAccess.DapperRepository;
using Microsoft.Extensions.Logging;
using System.Data;

namespace AuthService.Infrastructure.Repositories.RefreshTokenRepository
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IDapperRepository _dapperRepository;
        private readonly ILogger<RefreshTokenRepository> _logger;

        public RefreshTokenRepository(
            IDapperRepository dapperRepository,
            ILogger<RefreshTokenRepository> logger)
        {
            _dapperRepository = dapperRepository;
            _logger = logger;
        }

        public async Task CreateAsync(RefreshToken token)
        {
            try
            {
                await _dapperRepository.ExecuteAsync(
                    StoredProcedureNames.USP_CreateRefreshToken,
                    new
                    {
                        token.Id,
                        token.UserId,
                        token.TokenHash,
                        token.ExpiresAt
                    },
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to create refresh token for UserId: {UserId}",
                    token.UserId);

                throw;
            }
        }

        public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash)
        {
            try
            {
                return await _dapperRepository.QueryFirstOrDefaultAsync<RefreshToken>(
                    StoredProcedureNames.USP_GetRefreshTokenByHash,
                    new { TokenHash = tokenHash },
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to retrieve refresh token by hash");

                throw;
            }
        }

        public async Task RevokeAsync(Guid tokenId)
        {
            try
            {
                await _dapperRepository.ExecuteAsync(
                    StoredProcedureNames.USP_RevokeRefreshToken,
                    new { RefreshTokenId = tokenId },
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to revoke refresh token. TokenId: {TokenId}",
                    tokenId);

                throw;
            }
        }
    }
}
