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
    public async Task<ServiceResponse<ShoppingCartDto>> GetShoppingCartAsync(string? sub, string? sessionId)
    {
        Guid? userId = null;
        if (!string.IsNullOrEmpty(sub) && Guid.TryParse(sub, out var parsedId))
        {
            userId = parsedId;
        }

        // 1. Kiểm tra: Phải có ít nhất UserId hoặc SessionId
        if (userId == null && string.IsNullOrEmpty(sessionId))
        {
            return new ServiceResponse<ShoppingCartDto>
            {
                Status = 400,
                Message = "Không có thông tin định danh giỏ hàng."
            };
        }

        // 2. Xây dựng câu truy vấn linh hoạt (Ưu tiên User, sau đó đến Session)
        var query = appDbContext.ShoppingCarts.AsQueryable();
        if (userId.HasValue)
        {
            query = query.Where(x => x.Iduser == userId.Value);
        }
        else
        {
            query = query.Where(x => x.SessionId == sessionId);
        }

        // 3. Truy vấn và ánh xạ (Dùng toán tử 3 ngôi để EF Core có thể dịch sang SQL)
        var cart = await query
            .Select(x => new ShoppingCartDto
            {
                TotalPrice = x.TotalPrice,
                ShoppingCartItems = x.ShoppingCartItems.Select(item =>
                    item.IdcomboNavigation != null
                        // Nếu là Combo
                        ? new ShoppingCartItemDto
                        {
                            Id = item.IdshoppingCartItem,
                            Name = item.IdcomboNavigation.Name,
                            Price = item.UnitPrice,// Nên dùng UnitPrice lưu sẵn ở CartItem thay vì móc lại vào Product
                            Quantity = item.Quantity,
                            Products = item.Cartcomboproduct.Select(product => new CartComboProductDto
                            {
                                Id = product.Idproduct,
                                Name = product.IdproductNavigation.Name,
                                Image = product.IdproductNavigation.Image,
                                Size = product.IdsizeNavigation != null ? product.IdsizeNavigation.Name : "",
                                Color = product.IdcolorNavigation != null ? product.IdcolorNavigation.Name : "",
                                Quantity = product.Quantity
                            }).ToList()
                        }
                        // Nếu là Sản phẩm lẻ
                        : new ShoppingCartItemDto
                        {
                            Id = item.IdshoppingCartItem,
                            Name = item.IdproductNavigation!.Name,
                            Image = item.IdproductNavigation.Image,
                            Size = item.IdsizeNavigation != null ? item.IdsizeNavigation.Name : "",
                            Color = item.IdcolorNavigation != null ? item.IdcolorNavigation.Name : "",
                            Price = item.UnitPrice,
                            Quantity = item.Quantity
                        }).ToList()
            })
            .FirstOrDefaultAsync();// Phải có hàm này để chốt câu lệnh và lấy về 1 đối tượng

        // 4. Xử lý trường hợp chưa có giỏ hàng (Trả về giỏ hàng rỗng thay vì báo lỗi)
        if (cart == null)
        {
            return new ServiceResponse<ShoppingCartDto>
            {
                Status = 200,
                Message = "Giỏ hàng trống",
                Data = new ShoppingCartDto
                {
                    TotalPrice = 0,
                    ShoppingCartItems = new List<ShoppingCartItemDto>()
                }
            };
        }

        return new ServiceResponse<ShoppingCartDto>
        {
            Status = 200,
            Message = "Lấy giỏ hàng thành công",
            Data = cart
        };
    }

    public async Task<ServiceResponse> AddToCartAsync(ShoppingCartItemUpdateDto item, string? sub, string? sessionId)
    {
        Guid? userId = null;
        if (!string.IsNullOrEmpty(sub) && Guid.TryParse(sub, out var parsedId))
        {
            userId = parsedId;
        }

        if (userId != null)
        {
            var user = await appDbContext.ShoppingCarts.FirstOrDefaultAsync(x => x.Iduser == userId);
            if (user == null)
            {
                return new ServiceResponse<ShoppingCartDto>
                {
                    Status = 400,
                    Message = "Người dùng không tồn tại."
                };
            }
            
        }
    }

    public async Task<ServiceResponse> UpdateCartItemAsync(Guid cartItemId, string? userId, string? sessionId,
        ShoppingCartItemUpdateDto item)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> RemoveFromCartAsync(Guid cartItemId, string? userId, string? sessionId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> ClearCartAsync(string? userId, string? sessionId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> MergeCartAsync(string? userId, string? sessionId)
    {
        throw new NotImplementedException();
    }
}
