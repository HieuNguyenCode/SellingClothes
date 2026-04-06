using Services.Dto.Requests;
using Services.Dto.Responses;

namespace Services.Interfaces;

public interface IAuthService
{
    Task<ServiceResponse<AuthDto>> LoginAsync(LoginDto loginDto);
}
