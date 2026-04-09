using Services.Dto.Requests;
using Services.Dto.Responses;

namespace Services.Interfaces;

public interface ISaleService
{
    Task<ServiceResponse<List<SalesDto>>> GetAllSalesAsync(string? search, int? page, int? pageSize);
    Task<ServiceResponse<SaleDto>> GetSaleByIdAsync(Guid saleId);
    Task<ServiceResponse> CreateSaleAsync(SaleUpdateDto saleDto, string? userId);
    Task<ServiceResponse> UpdateSaleAsync(Guid saleId, SaleUpdateDto saleDto, string? userId);
    Task<ServiceResponse> DeleteSaleByIdAsync(Guid saleId, string? userId);
}
