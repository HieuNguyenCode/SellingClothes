using System.Security.Claims;
using API.Common;
using Microsoft.AspNetCore.Mvc;
using Services.Dto.Requests;
using Services.Interfaces;

namespace API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ComboController(IComboService comboService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetCombos([FromQuery] string? search, [FromQuery] int? page,
        [FromQuery] int? pageSize)
    {
        return Result(await comboService.GetCombosAsync(User.FindFirstValue(ClaimTypes.Role), search, page, pageSize));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetComboById(Guid id)
    {
        return Result(await comboService.GetComboByIdAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> AddCombo([FromForm] ComboUpdateDto combo)
    {
        return Result(await comboService.AddComboAsync(combo, User.FindFirstValue(ClaimTypes.NameIdentifier)));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCombo(Guid id, [FromForm] ComboUpdateDto combo)
    {
        return Result(await comboService.UpdateComboAsync(id, combo, User.FindFirstValue(ClaimTypes.NameIdentifier)));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCombo(Guid id)
    {
        return Result(await comboService.DeleteComboAsync(id));
    }
}
