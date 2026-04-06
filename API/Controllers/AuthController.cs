using API.Common;
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
}
