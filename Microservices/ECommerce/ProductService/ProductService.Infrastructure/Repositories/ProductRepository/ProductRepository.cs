using BuildingBlocks.ExceptionsHelper;
using DatabaseAccess.DapperRepository;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using ProductService.Core.Constants;
using ProductService.Core.DTO;

namespace ProductService.Infrastructure.Repositories.ProductRepository
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDapperRepository _dapperRepository;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(IDapperRepository dapperRepository, ILogger<ProductRepository> logger)
        {
            _dapperRepository = dapperRepository;
            _logger = logger;
        }

        public async Task<Guid> CreateProductAsync(ProductRequestDto product)
        {
            try
            {
                var parameters = new
                {
                    product.ProductName,
                    product.Description,
                    product.Price,
                    product.Quantity,
                    product.ImageUrl,
                    product.Title,
                    product.PhysicalPath,
                    product.CategoryId
                };

                return await _dapperRepository
                    .QueryFirstOrDefaultAsync<Guid>(
                        StoreProcedureNames.USP_CreateProduct,
                        parameters);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error while creating product");
                throw new DataAccessException("Database error while creating product", ex);
            }
        }


        public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync()
        {
            try
            {
                var result = await _dapperRepository.QueryListOrDefault<ProductResponseDto>("USP_GetAllProducts");
                return result ?? Enumerable.Empty<ProductResponseDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving all products.");
                throw new DataAccessException("Unexpected error occurred while retrieving products.", ex);
            }
        }

        public async Task<ProductResponseDto?> GetProductByIdAsync(Guid id)
        {
            try
            {
                var parameters = new { Id = id };
                var result = await _dapperRepository.QueryFirstOrDefaultAsync<ProductResponseDto>("USP_GetProductById", parameters);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving product by ID {ProductId}.", id);
                throw new DataAccessException($"Unexpected error occurred while retrieving product with ID {id}.", ex);
            }
        }
    }
}
