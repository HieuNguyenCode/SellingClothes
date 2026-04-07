using API.Common;
using API.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Dto.Requests;
using Services.Interfaces;

namespace API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : BaseController
{
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        return Result(await authService.LoginAsync(loginDto));
    }

    [HttpPatch("RefreshToken")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshTokenOld = Request.GetBearerToken();
        return Result(await authService.RefreshToken(refreshTokenOld));
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        return Result(await authService.RegisterAsync(registerDto));
    }
}