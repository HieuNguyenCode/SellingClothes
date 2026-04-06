using Services.Dto.Requests;
using Services.Dto.Responses;
using Services.Interfaces;

namespace Services.Features;

public class AuthService : IAuthService
{
    public async Task<ServiceResponse<AuthDto>> LoginAsync(LoginDto loginDto)
    {
        throw new NotImplementedException();
    }
}
