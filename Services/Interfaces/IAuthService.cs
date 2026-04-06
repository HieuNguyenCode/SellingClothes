using Services.Dto.Requests;
using Services.Dto.Responses;

namespace Services.Interfaces;

public interface IAuthService
{
    Task<ServiceResponse<AuthDto>> LoginAsync(LoginDto loginDto);
    Task<ServiceResponse<AuthDto>> RefreshToken(string refreshTokenOld);
    Task<ServiceResponse> RegisterAsync(RegisterDto registerDto);
}
