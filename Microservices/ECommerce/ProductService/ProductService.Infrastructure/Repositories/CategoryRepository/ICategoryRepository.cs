using ProductService.Core.Models;

namespace ProductService.Infrastructure.Repositories.CategoryRepository
{
    public interface ICategoryRepository
    {
        Task<int> CreateCategoryAsync(string categoryName);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
    }
}
