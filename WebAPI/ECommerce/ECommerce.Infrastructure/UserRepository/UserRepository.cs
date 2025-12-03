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

            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? configuration["ConnectionStrings:DefaultConnection"];

            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new InvalidOperationException("Database connection string 'DefaultConnection' is not configured.");
            }
        }

        public async Task AddUserAsync(Users user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            const string sql = @"
                INSERT INTO Users (
                    UserName,
                    Email,
                    Password,
                    PasswordSalt,
                    FirstName,
                    LastName
                ) VALUES (
                    @Username,
                    @Email,
                    @Password,
                    @PasswordSalt,
                    @FirstName,
                    @LastName
                );
                SELECT CAST(SCOPE_IDENTITY() AS int);
                ";

            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var newId = await connection.QuerySingleAsync<int>(sql, user);
                var idProp = user.GetType().GetProperty("UserId");
                if (idProp != null && idProp.CanWrite)
                {
                    idProp.SetValue(user, Convert.ChangeType(newId, idProp.PropertyType));
                }

                _logger.LogDebug("Inserted user with id {UserId}", newId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while inserting user");
                throw;
            }
        }

        public async Task DeleteUserAsync(int userId)
        {
            const string sql = "DELETE FROM Users WHERE Id = @UserId;";

            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var affected = await connection.ExecuteAsync(sql, new { UserId = userId });
                _logger.LogDebug("Deleted user {UserId}. Rows affected: {Rows}", userId, affected);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting user {UserId}", userId);
                throw;
            }
        }

        public async Task<Users> GetUserByIdAsync(int userId)
        {
            const string sql = "SELECT * FROM Users WHERE Id = @UserId;";

            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var user = await connection.QueryFirstOrDefaultAsync<Users>(sql, new { UserId = userId });
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching user by id {UserId}", userId);
                throw;
            }
        }

        public async Task<Users> GetUserByUserCredentialsAsync(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentNullException(nameof(userName));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));

            const string sql = "USP_GetUserByUserCredentials";

            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Assumes UserCredentials has Username and PasswordHash properties. Adjust if different.
                var user = await connection.QueryFirstOrDefaultAsync<Users>(sql, new { userName, password }, commandType: CommandType.StoredProcedure);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching user by credentials for username {Username}", userName);
                throw;
            }
        }

        public async Task UpdateUserAsync(Users user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            const string sql = @"
                        UPDATE Users
                        SET
                            UserName = @UserName,
                            Email = @Email,
                            Password = @Password,
                            PasswordSalt = @PasswordSalt,
                            FirstName = @FirstName,
                            LastName = @LastName
                        WHERE UserId = @UserId;
                        ";

            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var affected = await connection.ExecuteAsync(sql, user);
                _logger.LogDebug("Updated user {UserId}. Rows affected: {Rows}", user.GetType().GetProperty("Id")?.GetValue(user), affected);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating user");
                throw;
            }
        }
    }
}
