using Dapper;
using DatabaseAccess.DapperRepository;
using Microsoft.Extensions.Logging;
using ProductService.Core.Constants;
using ProductService.Core.Models;
using System.Data;

namespace ProductService.Infrastructure.Repositories.CategoryRepository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ILogger<CategoryRepository> _logger;
        private readonly IDapperRepository _dapperRepository;
        public CategoryRepository(ILogger<CategoryRepository> logger, IDapperRepository dapperRepository)
        {
            _logger = logger;
            _dapperRepository = dapperRepository;
        }
        public async Task<int> CreateCategoryAsync(string categoryName)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CategoryName", categoryName);
                var categoryId = await _dapperRepository.QueryFirstOrDefaultAsync<int>(StoreProcedureNames.USP_CreateCategory, parameters, commandType: CommandType.StoredProcedure);
                return categoryId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating category: {CategoryName}", categoryName);
                throw;
            }
           
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _dapperRepository.QueryListOrDefault<Category>(StoreProcedureNames.USP_GetAllCategories, commandType: CommandType.StoredProcedure);
                return categories ?? Enumerable.Empty<Category>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all categories.");
                throw;
            }
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CategoryId", id);
                var category = await _dapperRepository.QueryFirstOrDefaultAsync<Category>(StoreProcedureNames.USP_GetCategoryById, parameters, commandType: CommandType.StoredProcedure);
                return category;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving category by id: {CategoryId}", id);
                throw;
            }
        }
    }
}
