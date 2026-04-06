using API.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Dto.Requests;
using Services.Interfaces;

namespace API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class TypeController(ITypeService typeService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetType([FromQuery] string? search, [FromQuery] int? page,
        [FromQuery] int? pageSize)
    {
        return Result(await typeService.GetAllTypesAsync(search, page, pageSize));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTypeById(Guid id)
    {
        return Result(await typeService.GetTypeByIdAsync(id));
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> CreateType([FromBody] TypeUpdateDto typeCreateDto)
    {
        return Result(await typeService.CreateTypeAsync(typeCreateDto));
    }

    [Authorize(Roles = "admin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateType(Guid id, [FromBody] TypeUpdateDto typeUpdateDto)
    {
        return Result(await typeService.UpdateTypeAsync(id, typeUpdateDto));
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteType(Guid id)
    {
        return Result(await typeService.DeleteTypeAsync(id));
    }
}
