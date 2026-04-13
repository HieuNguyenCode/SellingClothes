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
    public async Task<IActionResult> GetShoppingCart()
    {
        return Result(await cartService.GetShoppingCartAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)));
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart([FromBody] ShoppingCartItemUpdateDto item
    )
    {
        return Result(await cartService.AddToCartAsync(item, User.FindFirstValue(ClaimTypes.NameIdentifier)));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCartItem(Guid id, [FromBody] ShoppingCartItemUpdateDto item
    )
    {
        return Result(await cartService.UpdateCartItemAsync(id, User.FindFirstValue(ClaimTypes.NameIdentifier), item));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveFromCart(Guid id)
    {
        return Result(await cartService.RemoveFromCartAsync(id, User.FindFirstValue(ClaimTypes.NameIdentifier)));
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        return Result(await cartService.ClearCartAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)));
    }

    [HttpPost("Merge")]
    public async Task<IActionResult> MergeCart()
    {
        return Result(await cartService.MergeCartAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)));
    }
}