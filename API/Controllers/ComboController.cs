using API.Common;
using Microsoft.AspNetCore.Mvc;
using Services.Dto.Requests;
using Services.Interfaces;

namespace API.Controllers;

public class ComboController(IComboService comboService) : BaseController
{
    [HttpGet]
    public IActionResult GetCombos([FromQuery] string? search, [FromQuery] int? page,
        [FromQuery] int? pageSize)
    {
        return Result(comboService.GetCombosAsync(search, page, pageSize));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetComboById(Guid id)
    {
        return Result(await comboService.GetComboByIdAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> AddCombo([FromBody] ComboUpdateDto combo)
    {
        return Result(await comboService.AddComboAsync(combo));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCombo(Guid id, [FromBody] ComboUpdateDto combo)
    {
        return Result(await comboService.UpdateComboAsync(id, combo));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCombo(Guid id)
    {
        return Result(await comboService.DeleteComboAsync(id));
    }
}