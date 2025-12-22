using ProductService.Core.DTO;
namespace ProductService.Infrastructure.Repositories.ProductRepository
{
    public interface IProductRepository
    {
        Task<Guid> CreateProductAsync(ProductRequestDto product);
        Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync();
        Task<ProductResponseDto?> GetProductByIdAsync(Guid id);
    }
}
