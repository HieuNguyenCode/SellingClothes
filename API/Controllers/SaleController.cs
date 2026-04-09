using System.Security.Claims;
using API.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Dto.Requests;
using Services.Interfaces;

namespace API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class SaleController(ISaleService saleService) : BaseController
{
    [Authorize(Roles = "admin")]
    [HttpGet]
    public async Task<IActionResult> GetSales([FromQuery] string? search, [FromQuery] int? page,
        [FromQuery] int? pageSize)
    {
        return Result(await saleService.GetAllSalesAsync(search, page, pageSize));
    }

    [Authorize(Roles = "admin")]
    [HttpGet("{saleId:guid}")]
    public async Task<IActionResult> GetSaleById(Guid saleId)
    {
        return Result(await saleService.GetSaleByIdAsync(saleId));
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> CreateSale([FromBody] SaleUpdateDto saleCreateDto)
    {
        return Result(await saleService.CreateSaleAsync(saleCreateDto, User.FindFirstValue(ClaimTypes.NameIdentifier)));
    }

    [Authorize(Roles = "admin")]
    [HttpPut("{saleId}")]
    public async Task<IActionResult> UpdateSale(Guid saleId, [FromBody] SaleUpdateDto saleUpdateDto)
    {
        return Result(await saleService.UpdateSaleAsync(saleId, saleUpdateDto, User.FindFirstValue(ClaimTypes.NameIdentifier)));
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("{saleId}")]
    public async Task<IActionResult> DeleteSale(Guid saleId)
    {
        return Result(await saleService.DeleteSaleByIdAsync(saleId, User.FindFirstValue(ClaimTypes.NameIdentifier)));
    }
}
