using Services.Dto.Requests;
using Services.Dto.Responses;

namespace Services.Interfaces;

public interface IComboService
{
    Task<ServiceResponse<List<CombosDto>>> GetCombosAsync(string? search, int? page, int? pageSize);
    Task<ServiceResponse<ComboDto>> GetComboByIdAsync(Guid id);
    Task<ServiceResponse> AddComboAsync(ComboUpdateDto combo);
    Task<ServiceResponse> UpdateComboAsync(Guid id, ComboUpdateDto combo);
    Task<ServiceResponse> DeleteComboAsync(Guid id);
}