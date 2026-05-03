using System.Security.Claims;
using API.Common;
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
}