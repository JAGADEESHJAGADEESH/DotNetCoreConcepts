using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DatabaseAccess.DapperRepository
{
    public class DapperRepository : IDapperRepository
    {
        private readonly string _connectionString;

        public DapperRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<int> ExecuteAsync(string storedProcedure, object? parameters = null, CommandType commandType = CommandType.StoredProcedure)
        {
            if (string.IsNullOrWhiteSpace(storedProcedure))
            {
                throw new ArgumentException("Stored procedure name must be provided.", nameof(storedProcedure));
            }

            var connection = await CreateConnectionAsync().ConfigureAwait(false);

            var affectedRows = await connection.ExecuteAsync(
                sql: storedProcedure,
                param: parameters,
                commandType: commandType
            ).ConfigureAwait(false);

            await DisposeConnectionAsync(connection);
            return affectedRows;
        }

        public async Task<List<T>?> QueryListOrDefault<T>(string storedProcedure, object? parameters = null, CommandType commandType = CommandType.StoredProcedure)
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
                commandType: commandType
            ).ConfigureAwait(false);

            var objectList = result?.ToList();

            await DisposeConnectionAsync(connection);

            return objectList;
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

        public async Task<T?> QueryFirstOrDefaultAsync<T>(string storedProcedure, object? parameters = null, CommandType commandType = CommandType.StoredProcedure)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync().ConfigureAwait(false);

            var result = await connection.QueryFirstOrDefaultAsync<T>(
                sql: storedProcedure,
                param: parameters,
                commandType: commandType
            ).ConfigureAwait(false);
            await DisposeConnectionAsync(connection);

            return result;
        }
    }
}
