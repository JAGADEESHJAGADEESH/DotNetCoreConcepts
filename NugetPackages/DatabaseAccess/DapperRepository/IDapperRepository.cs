using System.Data;

namespace DatabaseAccess.DapperRepository
{
    public interface IDapperRepository
    {
        Task<List<T>?> QueryListOrDefault<T>(string storedProcedure, object? parameters = null, CommandType commandType = CommandType.StoredProcedure);
        Task<int> ExecuteAsync(string storedProcedure, object? parameters = null, CommandType commandType = CommandType.StoredProcedure);
        Task<T?> QueryFirstOrDefaultAsync<T>(string storedProcedure, object? parameters = null, CommandType commandType = CommandType.StoredProcedure);
    }
}
