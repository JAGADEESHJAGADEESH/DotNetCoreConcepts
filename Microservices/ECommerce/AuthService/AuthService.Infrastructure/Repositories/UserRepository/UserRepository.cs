using AuthService.Core.Constants;
using AuthService.Core.Models;
using BuildingBlocks.ExceptionsHelper;
using DatabaseAccess.DapperRepository;
using Microsoft.Extensions.Logging;
using System.Data;

namespace AuthService.Infrastructure.Repositories.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;
        private readonly IDapperRepository _dapperRepository;
        public UserRepository(ILogger<UserRepository> logger, IDapperRepository dapperRepository)
        {
            _logger = logger;
            _dapperRepository = dapperRepository;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            try
            {
                return await _dapperRepository.QueryFirstOrDefaultAsync<User>(StoredProcedureNames.USP_GetUserByEmail,
                    new { Email = email }, commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user with email: {Email}", email);
                // Add context and rethrow
                throw new DataAccessException(
                    $"Failed to fetch user with email: {email}", ex);
            }
        }


        public async Task<bool> IsUserEmailExistsInDBAsync(string email)
        {
            try
            {
                var isValid = await _dapperRepository.QueryFirstOrDefaultAsync<bool>(StoredProcedureNames.USP_GetUserByEmail,
                    new { Email = email }, commandType: CommandType.StoredProcedure
                );

                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while validating user: {Email}", email);
                throw new DataAccessException($"Failed to validate user with email: {email}", ex);
            }
        }

        public async Task<bool> RegisterUserAsync(User user)
        {
            try
            {
                var parameters = new
                {
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.Username,
                    user.PasswordHash,
                    user.PasswordSalt
                };
                var userId = await _dapperRepository.ExecuteAsync(StoredProcedureNames.USP_RegisterUser,
                    parameters, commandType: CommandType.StoredProcedure);
                return userId > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering user: {Email}", user.Email);
                throw new DataAccessException($"Failed to register user with email: {user.Email}", ex);
            }
        }

    }
}
