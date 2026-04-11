using Services.Dto.Requests;
using Services.Dto.Responses;

namespace Services.Interfaces;

public interface IShoppingCartService
{
    Task<ServiceResponse<ShoppingCartDto>> GetShoppingCartAsync(string? userId, string? sessionId);
    Task<ServiceResponse> AddToCartAsync(ShoppingCartItemUpdateDto item, string? userId, string? sessionId);
    Task<ServiceResponse> UpdateCartItemAsync(Guid cartItemId, string? userId, string? sessionId, ShoppingCartItemUpdateDto item);
    Task<ServiceResponse> RemoveFromCartAsync(Guid cartItemId, string? userId, string? sessionId);
    Task<ServiceResponse> ClearCartAsync(string? userId, string? sessionId);
    Task<ServiceResponse> MergeCartAsync(string? userId, string? sessionId);
}
