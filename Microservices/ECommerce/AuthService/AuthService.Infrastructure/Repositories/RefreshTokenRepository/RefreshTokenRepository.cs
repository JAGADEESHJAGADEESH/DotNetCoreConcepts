using AuthService.Core.Constants;
using AuthService.Core.Models;
using BuildingBlocks.ExceptionsHelper;
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

                throw new DataAccessException(
                    "Error creating refresh token", ex);
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

                throw new DataAccessException(
                    "Error retrieving refresh token", ex);
            }
        }

        public async Task RevokeAsync(Guid tokenId)
        {
            try
            {
                await _dapperRepository.ExecuteAsync(
                    StoredProcedureNames.USP_RevokeRefreshToken,
                    new { TokenId = tokenId },   
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to revoke refresh token with id {TokenId}", tokenId);

                throw new DataAccessException(
                    "Error revoking refresh token", ex);
            }
        }

    }
}
