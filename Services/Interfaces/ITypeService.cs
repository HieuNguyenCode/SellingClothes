using Services.Dto.Requests;
using Services.Dto.Responses;

namespace Services.Interfaces;

public interface ITypeService
{
    Task<ServiceResponse<List<TypeDto>>> GetAllTypesAsync(string? search, int? page, int? pageSize);
    Task<ServiceResponse<TypeDto>> GetTypeByIdAsync(Guid id);
    Task<ServiceResponse> CreateTypeAsync(TypeUpdateDto typeDto);
    Task<ServiceResponse> UpdateTypeAsync(Guid id, TypeUpdateDto typeDto);
    Task<ServiceResponse> DeleteTypeAsync(Guid id);
}
