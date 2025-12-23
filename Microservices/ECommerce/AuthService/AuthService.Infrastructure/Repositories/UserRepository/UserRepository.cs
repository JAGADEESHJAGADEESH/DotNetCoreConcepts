using AuthService.Core.Constants;
using AuthService.Core.DTO;
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

        public async Task<AuthInfo?> GetUserAuthInfoAsync(string email)
        {
            try
            {
                return await _dapperRepository.QueryFirstOrDefaultAsync<AuthInfo>(StoredProcedureNames.USP_GetUserAuthInfo,
                    new { Email = email }, commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user with email: {Email}", email);
                throw new DataAccessException($"Failed to fetch user with email: {email}", ex);
            }
        }


        public async Task<Roles?> GetRoleByRoleIdAsync(int id)
        {
            try
            {
                return await _dapperRepository.QueryFirstOrDefaultAsync<Roles>(StoredProcedureNames.USP_GetRoleByRoleId,
                    new { RoleId = id }, commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user role: {Id}", id);
                throw new DataAccessException($"Failed to fetch user role with id: {id}", ex);
            }
        }

        public async Task<Guid> RegisterUserAsync(User user)
        {
            try
            {
                var parameters = new
                {
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    Username = $"{user.FirstName} {user.LastName}",
                    user.PasswordHash,
                    user.PasswordSalt,
                    RoleId = user.Role.Id,
                    user.IsActive,
                    user.CreatedAt
                };

                var userId = await _dapperRepository.QueryFirstOrDefaultAsync<Guid>(StoredProcedureNames.USP_RegisterUser,
                    parameters, commandType: CommandType.StoredProcedure);
                return userId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering user: {Email}", user.Email);
                throw new DataAccessException($"Failed to register user with email: {user.Email}", ex);
            }
        }

    }
}
