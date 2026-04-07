using Services.Dto.Requests;
using Services.Dto.Responses;

namespace Services.Interfaces;

public interface IProductService
{
    Task<ServiceResponse<List<ProductsDto>>> GetProductsAsync(string? search, int? page, int? pageSize);
    Task<ServiceResponse<ProductDto>> GetProductByIdAsync(Guid id);
    Task<ServiceResponse> CreateProductAsync(ProductUpdateDto productCreateDto, string? userId);
    Task<ServiceResponse> UpdateProductAsync(Guid id, ProductUpdateDto productUpdateDto, string? userId);
    Task<ServiceResponse> DeleteProductAsync(Guid id);
}