using Services.Dto.Requests;
using Services.Dto.Responses;
using Services.Interfaces;

namespace Services.Features;

public class SaleService : ISaleService
{
    public async Task<ServiceResponse<List<SalesDto>>> GetAllSalesAsync(string? search, int? page, int? pageSize)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<SaleDto>> GetSaleByIdAsync(Guid saleId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> CreateSaleAsync(SaleUpdateDto saleDto)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> UpdateSaleAsync(Guid saleId, SaleUpdateDto saleDto)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> DeleteSaleByIdAsync(Guid saleId)
    {
        throw new NotImplementedException();
    }
}