using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Dto.Requests;
using Services.Dto.Responses;
using Services.Interfaces;

namespace Services.Features;

public class ShoppingCartService(
    AppDbContext appDbContext,
    ILogger<ShoppingCartService> logger
) : IShoppingCartService
{
    public async Task<ServiceResponse<ShoppingCartDto>> GetShoppingCartAsync(string? sub)
    {
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId))
            return new ServiceResponse<ShoppingCartDto>
            {
                Status = 400,
                Message = "Thông tin người dùng không hợp lệ"
            };

        var shoppingCart = await appDbContext.ShoppingCarts
            .Where(cart => cart.Iduser == userId)
            .Select(cart => new ShoppingCartDto
            {
                TotalPrice = cart.TotalPrice,
                ShoppingCartItems = cart.ShoppingCartItems.Select(item =>
                    item.Idproduct != null
                        ? new ShoppingCartItemDto
                        {
                            Id = item.IdshoppingCartItem,
                            Name = item.IdproductNavigation!.Name,
                            Size = item.IdsizeNavigation!.Name,
                            Color = item.IdcolorNavigation!.Name,
                            Image = item.IdproductNavigation.Image,
                            Quantity = item.Quantity,
                            Price = item.IdproductNavigation.Price * item.Quantity
                        }
                        : new ShoppingCartItemDto
                        {
                            Id = item.IdshoppingCartItem,
                            Name = item.IdcomboNavigation!.Name,
                            Products = item.CartComboProducts.Select(cp => new CartComboProductDto
                            {
                                Name = cp.IdproductNavigation.Name,
                                Size = cp.IdsizeNavigation!.Name,
                                Color = cp.IdcolorNavigation!.Name,
                                Image = cp.IdproductNavigation.Image,
                                Quantity = cp.Quantity
                            }).ToList(),
                            Quantity = item.Quantity,
                            Price = item.IdcomboNavigation.Price * item.Quantity
                        }
                ).ToList()
            }).FirstOrDefaultAsync();

        return new ServiceResponse<ShoppingCartDto>
        {
            Status = 200,
            Message = "Lấy giỏ hàng thành công",
            Data = shoppingCart
        };
    }

    public async Task<ServiceResponse> AddToCartAsync(ShoppingCartItemUpdateDto item, string? sub)
    {
        
    }

    public async Task<ServiceResponse> UpdateCartItemAsync(Guid cartItemId, string? sub, ShoppingCartItemUpdateDto item)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> RemoveFromCartAsync(Guid cartItemId, string? sub)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> ClearCartAsync(string? sub)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> MergeCartAsync(string? sub)
    {
        throw new NotImplementedException();
    }
}