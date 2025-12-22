using Microsoft.Extensions.Logging;
using ProductService.Core.Models;
using ProductService.Infrastructure.Repositories.CategoryRepository;

namespace ProductService.Application.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly ILogger<CategoryService> _logger;
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ILogger<CategoryService> logger, ICategoryRepository categoryRepository)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
        }
        public async Task<int> CreateCategoryAsync(string categoryName)
        {
            _logger.LogInformation("Creating category: {CategoryName}", categoryName);
            try
            {
                var categoryId = await _categoryRepository.CreateCategoryAsync(categoryName);
                _logger.LogInformation("Category created with ID: {CategoryId}", categoryId);
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
            _logger.LogInformation("Retrieving all categories...");
            try
            {
                var categories = await _categoryRepository.GetAllCategoriesAsync();
                _logger.LogInformation("Retrieved {CategoryCount} categories.", categories.Count());
                return categories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all categories.");
                throw;
            }
        }
        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving category with ID: {CategoryId}", id);
            try
            {
                var category = await _categoryRepository.GetCategoryByIdAsync(id);
                if (category is null)
                {
                    _logger.LogWarning("Category with ID: {CategoryId} not found.", id);
                }
                return category;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving category with ID: {CategoryId}", id);
                throw;
            }
        }
    }
}
