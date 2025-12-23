using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Services.ProductCatelogService;
using ProductService.Core.DTO;

namespace ProductService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductCatelogService _productService;

        public ProductController(IProductCatelogService productService)
        {
            _productService = productService;
        }

        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProductsAsync()
        {
            var result = await _productService.GetAllProductsAsync();

            if (!result.Success)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("GetProductById/{id}")]
        public async Task<IActionResult> GetProductByIdAsync(Guid id)
        {
            var result = await _productService.GetProductByIdAsync(id);

            if (!result.Success)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProductAsync([FromBody]ProductRequestDto product)
        {
            var result = await _productService.CreateProductAsync(product);

            if (!result.Success)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }
    }

}
