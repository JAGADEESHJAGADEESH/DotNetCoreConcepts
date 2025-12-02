using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Infrastructure.DapperRepository
{
    public class DapperRepository : IDapperRepository
    {
        private readonly string _connectionString;

        public DapperRepository(IConfiguration configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var connStr = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connStr))
            {
                connStr = configuration["ConnectionStrings:DefaultConnection"];
            }

            if (string.IsNullOrWhiteSpace(connStr))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
            }

            _connectionString = connStr;
        }

        public async Task<int> ExecuteAsync(string storedProcedure, object? parameters = null)
        {
            if (string.IsNullOrWhiteSpace(storedProcedure))
            {
                throw new ArgumentException("Stored procedure name must be provided.", nameof(storedProcedure));
            }

            var connection = await CreateConnectionAsync().ConfigureAwait(false);

            var affectedRows = await connection.ExecuteAsync(
                sql: storedProcedure,
                param: parameters,
                commandType: CommandType.StoredProcedure
            ).ConfigureAwait(false);

            await DisposeConnectionAsync(connection);
            return affectedRows;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string storedProcedure, object? parameters = null)
        {
            if (string.IsNullOrWhiteSpace(storedProcedure))
            {
                throw new ArgumentException("Stored procedure name must be provided.", nameof(storedProcedure));
            }

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync().ConfigureAwait(false);

            var result = await connection.QueryAsync<T>(
                sql: storedProcedure,
                param: parameters,
                commandType: CommandType.StoredProcedure
            ).ConfigureAwait(false);
            await DisposeConnectionAsync(connection);

            return result;
        }

        private async Task<IDbConnection> CreateConnectionAsync()
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync().ConfigureAwait(false);
            return connection;
        }
        private static async Task DisposeConnectionAsync(IDbConnection connection)
        {
            if (connection is SqlConnection sqlConnection)
            {
                await sqlConnection.CloseAsync().ConfigureAwait(false);
                await sqlConnection.DisposeAsync().ConfigureAwait(false);
            }
            else
            {
                connection.Close();
                connection.Dispose();
            }
        }
    }
}
