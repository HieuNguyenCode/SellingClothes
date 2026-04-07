using System.Security.Claims;
using API.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Dto.Requests;
using Services.Interfaces;

namespace API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ProductController(IProductService productService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] string? search, [FromQuery] int? page,
        [FromQuery] int? pageSize)
    {
        return Result(await productService.GetProductsAsync(search, page, pageSize));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        return Result(await productService.GetProductByIdAsync(id));
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductUpdateDto productCreateDto)
    {
        return Result(await productService.CreateProductAsync(productCreateDto,
            User.FindFirstValue(ClaimTypes.NameIdentifier)));
    }

    [Authorize(Roles = "admin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] ProductUpdateDto productUpdateDto)
    {
        return Result(await productService.UpdateProductAsync(id, productUpdateDto,
            User.FindFirstValue(ClaimTypes.NameIdentifier)));
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        return Result(await productService.DeleteProductAsync(id));
    }
}