using Services.Dto.Requests;
using Services.Dto.Responses;

namespace Services.Interfaces;

public interface IShoppingCartService
{
    Task<ServiceResponse<ShoppingCartDto>> GetShoppingCartAsync(string? sub);
    Task<ServiceResponse> AddToCartAsync(ShoppingCartItemUpdateDto item, string? sub);

    Task<ServiceResponse> UpdateCartItemAsync(Guid cartItemId, string? sub,
        ShoppingCartItemUpdateDto item);

    Task<ServiceResponse> RemoveFromCartAsync(Guid cartItemId, string? sub);
    Task<ServiceResponse> ClearCartAsync(string? sub);
    Task<ServiceResponse> MergeCartAsync(string? sub);
}