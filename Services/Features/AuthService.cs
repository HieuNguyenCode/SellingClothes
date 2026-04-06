using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Services.Dto.Requests;
using Services.Dto.Responses;
using Services.Interfaces;

namespace Services.Features;

public class AuthService(
    AppDbContext appDbContext,
    ITokenService tokenService
) : IAuthService
{
    public async Task<ServiceResponse<AuthDto>> LoginAsync(LoginDto loginDto)
    {
        var user = await appDbContext.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
        {
            return new ServiceResponse<AuthDto>
            {
                Status = 401,
                Message = "Tên người dùng hoặc mật khẩu không đúng."
            };
        }

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken(user);
        return new ServiceResponse<AuthDto>
        {
            Status = 200,
            Message = "Đăng nhập thành công.",
            Data = new AuthDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            }
        };
    }

    public async Task<ServiceResponse<AuthDto>> RefreshToken(string refreshTokenOld)
    {
        if (string.IsNullOrEmpty(refreshTokenOld))
        {
            return new ServiceResponse<AuthDto>
            {
                Status = 400,
                Message = "Refresh token không được để trống"
            };
        }

        var sub = tokenService.GetSubFromToken(refreshTokenOld);
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var idUser))
        {
            return new ServiceResponse<AuthDto>
            {
                Status = 400,
                Message = "Token JTI lỗi format"
            };
        }

        var user = await appDbContext.Users.FirstOrDefaultAsync(u => u.Iduser == idUser);
        if (user == null)
        {
            return new ServiceResponse<AuthDto>
            {
                Status = 401,
                Message = "Người dùng không tồn tại"
            };
        }

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken(user);
        return new ServiceResponse<AuthDto>
        {
            Status = 200,
            Message = "Đăng nhập thành công.",
            Data = new AuthDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            }
        };
    }

    public async Task<ServiceResponse> RegisterAsync(RegisterDto registerDto)
    {
        var user = await appDbContext.Users.FirstOrDefaultAsync(u => u.UserName == registerDto.Username);
        if (user != null)
        {
            return new ServiceResponse
            {
                Status = 400,
                Message = "Tên người dùng đã tồn tại."
            };
        }

        var newUser = new User
        {
            UserName = registerDto.Username,
            Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
        };
        await appDbContext.Users.AddAsync(newUser);
        await appDbContext.SaveChangesAsync();
        return new ServiceResponse
        {
            Status = 200,
            Message = "Đăng ký thành công."
        };
    }
}
