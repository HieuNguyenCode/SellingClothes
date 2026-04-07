using API.Common;
using Microsoft.AspNetCore.Mvc;
using Services.Dto.Requests;
using Services.Interfaces;

namespace API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class SaleController(ISaleService saleService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetSales([FromQuery] string? search, [FromQuery] int? page,
        [FromQuery] int? pageSize)
    {
        return Result(await saleService.GetAllSalesAsync(search, page, pageSize));
    }

    [HttpGet("{saleId}")]
    public async Task<IActionResult> GetSaleById(Guid saleId)
    {
        return Result(await saleService.GetSaleByIdAsync(saleId));
    }

    [HttpPost]
    public async Task<IActionResult> CreateSale([FromBody] SaleUpdateDto saleCreateDto)
    {
        return Result(await saleService.CreateSaleAsync(saleCreateDto));
    }

    [HttpPut("{saleId}")]
    public async Task<IActionResult> UpdateSale(Guid saleId, [FromBody] SaleUpdateDto saleUpdateDto)
    {
        return Result(await saleService.UpdateSaleAsync(saleId, saleUpdateDto));
    }

    [HttpDelete("{saleId}")]
    public async Task<IActionResult> DeleteSale(Guid saleId)
    {
        return Result(await saleService.DeleteSaleByIdAsync(saleId));
    }
}