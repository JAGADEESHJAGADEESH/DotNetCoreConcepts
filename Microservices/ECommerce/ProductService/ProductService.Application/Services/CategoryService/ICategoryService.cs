using ProductService.Core.Models;

namespace ProductService.Application.Services.CategoryService
{
    public interface ICategoryService
    {
        Task<int> CreateCategoryAsync(string categoryName);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
    }
}
