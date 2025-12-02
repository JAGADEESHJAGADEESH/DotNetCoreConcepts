namespace ECommerce.Infrastructure.DapperRepository
{
    public interface IDapperRepository
    {
        Task<IEnumerable<T>> QueryAsync<T>(string storedProcedure, object? parameters = null);
        Task<int> ExecuteAsync(string storedProcedure, object? parameters = null);

    }
}
