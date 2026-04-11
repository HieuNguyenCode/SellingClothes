using System.Security.Claims;
using API.Common;
using Microsoft.AspNetCore.Mvc;
using Services.Dto.Requests;
using Services.Interfaces;

namespace API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ShoppingCartController(IShoppingCartService cartService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetShoppingCart([FromHeader(Name = "X-Session-ID")] string? sessionId)
    {
        return Result(await cartService.GetShoppingCartAsync(User.FindFirstValue(ClaimTypes.NameIdentifier), sessionId));
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart([FromBody] ShoppingCartItemUpdateDto item,
        [FromHeader(Name = "X-Session-ID")] string? sessionId)
    {
        return Result(await cartService.AddToCartAsync(item, User.FindFirstValue(ClaimTypes.NameIdentifier), sessionId));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCartItem(Guid id, [FromBody] ShoppingCartItemUpdateDto item,
        [FromHeader(Name = "X-Session-ID")] string? sessionId)
    {
        return Result(await cartService.UpdateCartItemAsync(id, User.FindFirstValue(ClaimTypes.NameIdentifier), sessionId, item));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveFromCart(Guid id, [FromHeader(Name = "X-Session-ID")] string? sessionId)
    {
        return Result(await cartService.RemoveFromCartAsync(id, User.FindFirstValue(ClaimTypes.NameIdentifier), sessionId));
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart([FromHeader(Name = "X-Session-ID")] string? sessionId)
    {
        return Result(await cartService.ClearCartAsync(User.FindFirstValue(ClaimTypes.NameIdentifier), sessionId));
    }

    [HttpPost("Merge")]
    public async Task<IActionResult> MergeCart([FromHeader(Name = "X-Session-ID")] string? sessionId)
    {
        return Result(await cartService.MergeCartAsync(User.FindFirstValue(ClaimTypes.NameIdentifier), sessionId));
    }
}
