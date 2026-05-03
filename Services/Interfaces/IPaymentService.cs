using Services.Dto.Requests;
using Services.Dto.Responses;

namespace Services.Interfaces;

public interface IPaymentService
{
    Task<ServiceResponse> AddProductToCartAsync(ShoppingCartUpdateDto item, string? sub);
}