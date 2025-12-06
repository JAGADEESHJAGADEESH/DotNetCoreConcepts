using Dapper;
using Ecommerce.Core.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;

namespace ECommerce.Infrastructure.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;
        private readonly string _connectionString;

        public UserRepository(ILogger<UserRepository> logger, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (configuration is not null)
            {
                var configSection = configuration.GetSection("ConnectionStrings");
                if (configSection is not null)
                {
                    var connectionString = configSection.GetConnectionString("DefaultConnection");
                    if (!string.IsNullOrWhiteSpace(connectionString))
                        _connectionString = connectionString;
                }
            }

            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new InvalidOperationException("Database connection string 'DefaultConnection' is not configured.");
            }
        }

        public async Task SaveUserAsync(Users user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            const string sql = @"[dbo].[USP_SaveUser]";

            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var newId = await connection.ExecuteAsync(sql, user);

                _logger.LogDebug("Inserted user with id {UserId}", newId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while inserting user");
                throw;
            }
        }

      
        public async Task<Users> GetUserByUserCredentialsAsync(string userName, string passwordSalt)
        {
            if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentNullException(nameof(userName));
            if (string.IsNullOrWhiteSpace(passwordSalt)) throw new ArgumentNullException(nameof(passwordSalt));

            const string sql = "USP_GetUserByUserCredentials";

            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Assumes UserCredentials has Username and PasswordHash properties. Adjust if different.
                var user = await connection.QueryFirstOrDefaultAsync<Users>(sql, new { userName, passwordSalt }, commandType: CommandType.StoredProcedure);
                return user ?? new Users();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching user by credentials for username {Username}", userName);
                throw;
            }
        }
    }
}
