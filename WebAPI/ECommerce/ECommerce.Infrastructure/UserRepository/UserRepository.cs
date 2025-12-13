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
            var connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection");
            if (connectionString is not null && connectionString.Value is not null)
                _connectionString = connectionString.Value;

            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new InvalidOperationException("Database connection string 'DefaultConnection' is not configured.");
            }
        }

        public async Task<int?> SaveUserAsync(Users user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            const string sql = "USP_SaveUser";

            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var parameters = new
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    PasswordSalt = user.PasswordSalt,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };

                // ExecuteScalarAsync<int> reads the scalar value returned by the stored procedure (NewUserId).
                var newId = await connection.ExecuteScalarAsync<int>(sql, parameters, commandType: CommandType.StoredProcedure);
                if(newId <= 0)
                {
                    _logger.LogWarning("Failed to insert user, stored procedure returned invalid UserId: {UserId}", newId);
                    throw new InvalidOperationException("Failed to insert user, invalid UserId returned.");
                }

                _logger.LogDebug("Inserted user with id {UserId}", newId);
                return newId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while inserting user");
                throw;
            }
        }


        public async Task<Users> GetUserByUserCredentialsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));

            const string sql = "USP_GetUserByUserCredentials";

            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Assumes UserCredentials has Username and PasswordHash properties. Adjust if different.
                var user = await connection.QueryFirstOrDefaultAsync<Users>(sql, new { email }, commandType: CommandType.StoredProcedure);
                return user ?? null!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching user by credentials for username {Username}", email);
                throw;
            }
        }
    }
}
