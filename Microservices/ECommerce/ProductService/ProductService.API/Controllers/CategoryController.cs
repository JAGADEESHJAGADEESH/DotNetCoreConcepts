using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Services.CategoryService;

namespace ProductService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly ICategoryService _categoryService;

        public CategoryController(ILogger<CategoryController> logger, ICategoryService categoryService)
        {
            _logger = logger;
            _categoryService = categoryService;
        }

        [HttpGet("AllCategories")]
        public async Task<IActionResult> GetAllCategoriesAsync()
        {
            _logger.LogInformation("Get all categories request received.");
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpPost("CreateCategory")]
        public async Task<IActionResult> CreateCategoryAsync([FromBody] string categoryName)
        {
            _logger.LogInformation("Create category request received.");
            var categoryId = await _categoryService.CreateCategoryAsync(categoryName);
            var message = $"Category '{categoryName}' created with ID: {categoryId}";
            return Ok(message);
        }

        [HttpGet("CategoryById/{id}")]
        public async Task<IActionResult> GetCategoryByIdAsync(int id)
        {
            _logger.LogInformation("Get category by ID request received for ID: {CategoryId}", id);
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Category with ID: {CategoryId} not found.", id);
                return NotFound();
            }
            return Ok(category);
        }
    }
}
