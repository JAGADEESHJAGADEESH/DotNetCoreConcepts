using BuildingBlocks.Common;
using ProductService.Core.DTO;
using ProductService.Core.Models;

namespace ProductService.Application.Services.ProductCatelogService
{
    public interface IProductCatelogService
    {
        Task<Result<IEnumerable<Product>>> GetAllProductsAsync();
        Task<Result<Product>> GetProductByIdAsync(Guid id);
        Task<Result<Guid>> CreateProductAsync(ProductRequestDto product);
    }

}

