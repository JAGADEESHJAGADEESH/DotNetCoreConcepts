using BuildingBlocks.Common;
using Microsoft.Extensions.Logging;
using ProductService.Application.Services.CategoryService;
using ProductService.Core.DTO;
using ProductService.Core.Models;
using ProductService.Infrastructure.Repositories.ProductRepository;

namespace ProductService.Application.Services.ProductCatelogService
{
    public class ProductCatelogService : IProductCatelogService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryService _categoryService;

        public ProductCatelogService(IProductRepository productRepository, ICategoryService categoryService)
        {
            _productRepository = productRepository;
            _categoryService = categoryService;
        }

        public async Task<Result<Guid>> CreateProductAsync(ProductRequestDto product)
        {
            var productId = await _productRepository.CreateProductAsync(product);

            if (productId == Guid.Empty)
                return Result<Guid>.Fail("Product creation failed");

            return Result<Guid>.Ok(productId);
        }

        public async Task<Result<IEnumerable<Product>>> GetAllProductsAsync()
        {
            var productResponses = await _productRepository.GetAllProductsAsync();

            if (productResponses == null || !productResponses.Any())
                return Result<IEnumerable<Product>>.Fail("No products found");

            var products = await Task.WhenAll(productResponses.Select(MapToProduct));
            return Result<IEnumerable<Product>>.Ok(products);
        }

        public async Task<Result<Product>> GetProductByIdAsync(Guid id)
        {
            var productResponse = await _productRepository.GetProductByIdAsync(id);

            if (productResponse == null)
                return Result<Product>.Fail("Product not found");

            var product = await MapToProduct(productResponse);
            return Result<Product>.Ok(product);
        }

        private async Task<Product> MapToProduct(ProductResponseDto productResponse)
        {
            return new Product
            {
                Id = productResponse.Id,
                ProductName = productResponse.ProductName,
                Description = productResponse.Description,
                Price = productResponse.Price,
                Quantity = productResponse.Quantity,
                InStock = productResponse.InStock,
                CreatedDate = productResponse.CreatedDate,
                ModifiedDate = productResponse.ModifiedDate,
                Image = new ProductImage
                {
                    ImageUrl = productResponse.ImageUrl,
                    CreatedDate = productResponse.ImageCreatedDate,
                    ModifiedDate = productResponse.ImageModifiedDate,
                    Title = productResponse.Title,
                    PhysicalPath = productResponse.PhysicalPath
                },
                Category = await _categoryService.GetCategoryByIdAsync(productResponse.CategoryId ?? 0)
                            ?? new Category()
            };
        }
    }

}
