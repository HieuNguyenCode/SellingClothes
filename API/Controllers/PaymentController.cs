using System.Security.Claims;
using API.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Dto.Requests;
using Services.Interfaces;

namespace API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class PaymentController(IPaymentService paymentService) : BaseController
{
    [HttpPost]
    public async Task<IActionResult> AddProduct([FromBody] ShoppingCartUpdateDto item)
    {
        return Result(await paymentService.AddProductToCartAsync(item, User.FindFirstValue(ClaimTypes.NameIdentifier)));
    }

    [HttpGet("user/orders")]
    public async Task<IActionResult> GetUserOrders([FromQuery] string? sessionId, [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        return Result(await paymentService.GetUserOrdersAsync(User.FindFirstValue(ClaimTypes.NameIdentifier), sessionId,
            page, pageSize));
    }

    [Authorize(Roles = "admin")]
    [HttpGet("admin/orders")]
    public async Task<IActionResult> GetAllOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null)
    {
        return Result(await paymentService.GetAllOrdersForAdminAsync(page, pageSize, status));
    }

    [Authorize(Roles = "admin")]
    [HttpPatch("admin/orders/{orderId:guid}/status")]
    public async Task<IActionResult> UpdateOrderStatus(Guid orderId, [FromBody] OrderStatusUpdateDto updateDto)
    {
        return Result(await paymentService.UpdateOrderStatusAsync(orderId, updateDto,
            User.FindFirstValue(ClaimTypes.NameIdentifier)));
    }
}