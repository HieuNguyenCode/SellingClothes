using Core.Entities;
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
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId))
            return new ServiceResponse
            {
                Status = 400,
                Message = "Thông tin người dùng không hợp lệ"
            };

        await using var transaction = await appDbContext.Database.BeginTransactionAsync();
        try
        {
            // 1. Ensure the user has a ShoppingCart (Get or Create)
            var cart = await appDbContext.ShoppingCarts.FirstOrDefaultAsync(c => c.Iduser == userId);
            if (cart == null)
            {
                cart = new ShoppingCart { IdshoppingCart = Guid.NewGuid(), Iduser = userId };
                await appDbContext.ShoppingCarts.AddAsync(cart);
                await appDbContext.SaveChangesAsync(); // Save to generate the Cart ID
            }

            var quantityToAdd = item.Quantity ?? 1;

            if (item.IsCombo)
            {
                // --- COMBO LOGIC ---
                var combo = await appDbContext.Combos
                    .Include(c => c.ComboProducts)
                    .ThenInclude(cp => cp.IdproductNavigation)
                    .ThenInclude(product => product.Sizes)
                    .Include(c => c.ComboProducts)
                    .ThenInclude(cp => cp.IdproductNavigation)
                    .ThenInclude(product => product.Colors)
                    .FirstOrDefaultAsync(c => c.Name == item.Name && !c.IsDeleted && c.IsPublished);

                if (combo == null)
                    return new ServiceResponse { Status = 400, Message = "Combo không tồn tại hoặc ngừng bán" };

                // Pre-calculate the exact configuration the user requested
                var desiredComboProducts = new List<CartComboProduct>();
                foreach (var comboProd in combo.ComboProducts)
                {
                    var reqProd = item.Products?.FirstOrDefault(p => p.Name == comboProd.IdproductNavigation.Name);
                    Guid? selectedColorId = null;
                    Guid? selectedSizeId = null;

                    if (reqProd != null)
                    {
                        // Look up Color/Size directly from the included navigation properties (Avoids N+1 DB queries)
                        if (!string.IsNullOrEmpty(reqProd.Color))
                            selectedColorId = comboProd.IdproductNavigation.Colors
                                .FirstOrDefault(c => c.Name == reqProd.Color)?.Idcolor;

                        if (!string.IsNullOrEmpty(reqProd.Size))
                            selectedSizeId = comboProd.IdproductNavigation.Sizes
                                .FirstOrDefault(s => s.Name == reqProd.Size)?.Idsize;
                    }

                    desiredComboProducts.Add(new CartComboProduct
                    {
                        Idproduct = comboProd.Idproduct,
                        Quantity = comboProd.Quantity, // Quantity of this product inside the combo
                        Idcolor = selectedColorId,
                        Idsize = selectedSizeId
                    });
                }

                // Fetch existing cart items for this specific Combo
                var existingCartItems = await appDbContext.ShoppingCartItems
                    .Include(ci => ci.CartComboProducts)
                    .Where(ci => ci.IdshoppingCart == cart.IdshoppingCart && ci.Idcombo == combo.Idcombo)
                    .ToListAsync();

                // Check if there is an existing cart item with the EXACT same Color/Size configuration
                var matchedItem = existingCartItems.FirstOrDefault(ex =>
                    ex.CartComboProducts.Count == desiredComboProducts.Count &&
                    ex.CartComboProducts.All(exCp => desiredComboProducts.Any(dCp =>
                        dCp.Idproduct == exCp.Idproduct &&
                        dCp.Idcolor == exCp.Idcolor &&
                        dCp.Idsize == exCp.Idsize))
                );

                if (matchedItem != null)
                {
                    // Combo configuration exists, just update quantity
                    matchedItem.Quantity += quantityToAdd;
                    appDbContext.ShoppingCartItems.Update(matchedItem);
                }
                else
                {
                    // Combo configuration is new, add as a new row
                    var newCartItemId = Guid.NewGuid();
                    var newCartItem = new ShoppingCartItem
                    {
                        IdshoppingCartItem = newCartItemId,
                        IdshoppingCart = cart.IdshoppingCart,
                        Idcombo = combo.Idcombo,
                        Quantity = quantityToAdd,
                        UnitPrice = combo.Price
                    };

                    await appDbContext.ShoppingCartItems.AddAsync(newCartItem);

                    // Add links for combo products
                    foreach (var dCp in desiredComboProducts)
                    {
                        dCp.IdcartComboProduct = Guid.NewGuid();
                        dCp.IdshoppingCartItem = newCartItemId;
                        dCp.CreateBy = userId;
                        dCp.CreateAt = DateTime.Now;
                    }

                    await appDbContext.CartComboProducts.AddRangeAsync(desiredComboProducts);
                }
            }
            else
            {
                // --- SINGLE PRODUCT LOGIC ---
                var product = await appDbContext.Products
                    .Include(p => p.Colors)
                    .Include(p => p.Sizes)
                    .FirstOrDefaultAsync(p => p.Name == item.Name && !p.IsDeleted && p.IsPublished);

                if (product == null)
                    return new ServiceResponse { Status = 400, Message = "Sản phẩm không tồn tại hoặc ngừng bán" };

                Guid? selectedColorId = null;
                if (!string.IsNullOrEmpty(item.Color))
                    selectedColorId = product.Colors.FirstOrDefault(c => c.Name == item.Color)?.Idcolor;

                Guid? selectedSizeId = null;
                if (!string.IsNullOrEmpty(item.Size))
                    selectedSizeId = product.Sizes.FirstOrDefault(s => s.Name == item.Size)?.Idsize;

                // Check if this exact Product + Color + Size exists in the cart
                var matchedItem = await appDbContext.ShoppingCartItems
                    .FirstOrDefaultAsync(ci =>
                        ci.IdshoppingCart == cart.IdshoppingCart &&
                        ci.Idproduct == product.Idproduct &&
                        ci.Idcolor == selectedColorId &&
                        ci.Idsize == selectedSizeId);

                if (matchedItem != null)
                {
                    matchedItem.Quantity += quantityToAdd;
                    appDbContext.ShoppingCartItems.Update(matchedItem);
                }
                else
                {
                    var newCartItem = new ShoppingCartItem
                    {
                        IdshoppingCartItem = Guid.NewGuid(),
                        IdshoppingCart = cart.IdshoppingCart,
                        Idproduct = product.Idproduct,
                        Idcolor = selectedColorId,
                        Idsize = selectedSizeId,
                        Quantity = quantityToAdd,
                        UnitPrice = product.Price,
                        CreateBy = userId,
                        CreateAt = DateTime.Now
                    };
                    await appDbContext.ShoppingCartItems.AddAsync(newCartItem);
                }
            }

            await appDbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ServiceResponse
            {
                Status = 200,
                Message = "Thêm vào giỏ hàng thành công"
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            logger.LogError(ex, "Lỗi khi thêm vào giỏ hàng");
            return new ServiceResponse
            {
                Status = 500,
                Message = "Đã xảy ra lỗi khi thêm vào giỏ hàng"
            };
        }
    }

    public async Task<ServiceResponse> UpdateCartItemAsync(Guid cartItemId, string? sub, ShoppingCartItemUpdateDto item)
    {
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId))
            return new ServiceResponse
            {
                Status = 400,
                Message = "Thông tin người dùng không hợp lệ"
            };

        await using var transaction = await appDbContext.Database.BeginTransactionAsync();
        try
        {
            // 1. Lấy thông tin món hàng cần update (Bao gồm cả các sản phẩm con nếu là combo)
            var cartItem = await appDbContext.ShoppingCartItems
                .Include(ci => ci.CartComboProducts)
                .ThenInclude(ccp => ccp.IdproductNavigation)
                .FirstOrDefaultAsync(ci =>
                    ci.IdshoppingCartItem == cartItemId &&
                    ci.IdshoppingCartNavigation.Iduser == userId);

            if (cartItem == null)
                return new ServiceResponse { Status = 404, Message = "Không tìm thấy sản phẩm trong giỏ hàng" };

            // 2. Tính năng mở rộng: Nếu Update số lượng về <= 0, ta tự động xóa sản phẩm đó
            if (item.Quantity.HasValue && item.Quantity.Value <= 0)
            {
                if (cartItem.CartComboProducts.Any())
                    appDbContext.CartComboProducts.RemoveRange(cartItem.CartComboProducts);

                appDbContext.ShoppingCartItems.Remove(cartItem);

                await appDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ServiceResponse
                    { Status = 200, Message = "Đã xóa sản phẩm khỏi giỏ hàng do số lượng bằng 0" };
            }

            // 3. Cập nhật số lượng
            if (item.Quantity.HasValue) cartItem.Quantity = item.Quantity.Value;

            // 4. Xử lý thay đổi Color/Size cho SẢN PHẨM LẺ
            if (!item.IsCombo && cartItem.Idproduct != null)
            {
                var productColors =
                    await appDbContext.Colors.Where(c => c.Idproduct == cartItem.Idproduct).ToListAsync();
                var productSizes = await appDbContext.Sizes.Where(s => s.Idproduct == cartItem.Idproduct).ToListAsync();

                var newColorId = string.IsNullOrEmpty(item.Color)
                    ? null
                    : productColors.FirstOrDefault(c => c.Name == item.Color)?.Idcolor;
                var newSizeId = string.IsNullOrEmpty(item.Size)
                    ? null
                    : productSizes.FirstOrDefault(s => s.Name == item.Size)?.Idsize;

                // Kiểm tra trùng lặp: Nếu đổi màu/size thành giống y hệt một món ĐANG CÓ SẴN trong giỏ
                var duplicateItem = await appDbContext.ShoppingCartItems
                    .FirstOrDefaultAsync(ci =>
                        ci.IdshoppingCart == cartItem.IdshoppingCart &&
                        ci.IdshoppingCartItem != cartItem.IdshoppingCartItem && // Bỏ qua chính nó
                        ci.Idproduct == cartItem.Idproduct &&
                        ci.Idcolor == newColorId &&
                        ci.Idsize == newSizeId);

                if (duplicateItem != null)
                {
                    // Gộp (Merge): Cộng dồn số lượng vào món cũ và xóa món hiện tại đi
                    duplicateItem.Quantity += cartItem.Quantity;
                    duplicateItem.UpdateAt = DateTime.Now;
                    duplicateItem.UpdateBy = userId;

                    appDbContext.ShoppingCartItems.Update(duplicateItem);
                    appDbContext.ShoppingCartItems.Remove(cartItem);
                }
                else
                {
                    // Không trùng: Cập nhật trực tiếp lên dòng hiện tại
                    cartItem.Idcolor = newColorId;
                    cartItem.Idsize = newSizeId;
                    cartItem.UpdateAt = DateTime.Now;
                    cartItem.UpdateBy = userId;
                    appDbContext.ShoppingCartItems.Update(cartItem);
                }
            }
            // 5. Xử lý thay đổi Color/Size cho COMBO
            else if (item.IsCombo && cartItem.Idcombo != null && item.Products != null && item.Products.Any())
            {
                // Lấy danh sách ID của tất cả sản phẩm nằm trong combo này
                var productIdsInCombo = cartItem.CartComboProducts.Select(ccp => ccp.Idproduct).ToList();

                // Kéo toàn bộ màu và size của các sản phẩm đó lên (để tránh N+1 Query trong vòng lặp)
                var allComboColors = await appDbContext.Colors.Where(c => productIdsInCombo.Contains(c.Idproduct))
                    .ToListAsync();
                var allComboSizes = await appDbContext.Sizes.Where(s => productIdsInCombo.Contains(s.Idproduct))
                    .ToListAsync();

                foreach (var innerProduct in cartItem.CartComboProducts)
                {
                    var updateData = item.Products.FirstOrDefault(p => p.Name == innerProduct.IdproductNavigation.Name);
                    if (updateData != null)
                    {
                        var newColorId = string.IsNullOrEmpty(updateData.Color)
                            ? null
                            : allComboColors.FirstOrDefault(c =>
                                c.Idproduct == innerProduct.Idproduct && c.Name == updateData.Color)?.Idcolor;

                        var newSizeId = string.IsNullOrEmpty(updateData.Size)
                            ? null
                            : allComboSizes.FirstOrDefault(s =>
                                s.Idproduct == innerProduct.Idproduct && s.Name == updateData.Size)?.Idsize;

                        innerProduct.Idcolor = newColorId;
                        innerProduct.Idsize = newSizeId;
                        innerProduct.UpdateAt = DateTime.Now;
                        innerProduct.UpdateBy = userId;
                    }
                }

                cartItem.UpdateAt = DateTime.Now;
                cartItem.UpdateBy = userId;
                appDbContext.ShoppingCartItems.Update(cartItem);
            }

            await appDbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ServiceResponse
            {
                Status = 200,
                Message = "Cập nhật giỏ hàng thành công"
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            logger.LogError(ex, "Lỗi khi cập nhật sản phẩm trong giỏ hàng. CartItemId: {CartItemId}", cartItemId);
            return new ServiceResponse
            {
                Status = 500,
                Message = "Đã xảy ra lỗi khi cập nhật giỏ hàng"
            };
        }
    }

    public async Task<ServiceResponse> RemoveFromCartAsync(Guid cartItemId, string? sub)
    {
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId))
            return new ServiceResponse
            {
                Status = 400,
                Message = "Thông tin người dùng không hợp lệ"
            };

        await using var transaction = await appDbContext.Database.BeginTransactionAsync();
        try
        {
            // Lấy chi tiết món hàng trong giỏ, kèm theo liên kết tới Combo (nếu có)
            // Đồng thời kiểm tra bảo mật: Món hàng này phải thuộc về giỏ hàng của user đang request
            var cartItem = await appDbContext.ShoppingCartItems
                .Include(ci => ci.CartComboProducts)
                .FirstOrDefaultAsync(ci =>
                    ci.IdshoppingCartItem == cartItemId &&
                    ci.IdshoppingCartNavigation.Iduser == userId);

            if (cartItem == null)
                return new ServiceResponse
                {
                    Status = 404,
                    Message = "Không tìm thấy sản phẩm trong giỏ hàng hoặc bạn không có quyền xóa"
                };

            // Xóa các sản phẩm con trong Combo trước (nếu đây là một Combo)
            // Dù database có cấu hình Cascade Delete hay không thì việc xóa tường minh bằng EF Core vẫn an toàn hơn.
            if (cartItem.CartComboProducts.Any())
                appDbContext.CartComboProducts.RemoveRange(cartItem.CartComboProducts);

            // Xóa món hàng khỏi giỏ
            appDbContext.ShoppingCartItems.Remove(cartItem);

            await appDbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ServiceResponse
            {
                Status = 200,
                Message = "Xóa sản phẩm khỏi giỏ hàng thành công"
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            logger.LogError(ex, "Lỗi khi xóa sản phẩm khỏi giỏ hàng. CartItemId: {CartItemId}", cartItemId);
            return new ServiceResponse
            {
                Status = 500,
                Message = "Đã xảy ra lỗi khi xử lý yêu cầu xóa khỏi giỏ hàng"
            };
        }
    }

    public async Task<ServiceResponse> ClearCartAsync(string? sub)
    {
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId))
            return new ServiceResponse<ShoppingCartDto>
            {
                Status = 400,
                Message = "Thông tin người dùng không hợp lệ"
            };
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse> MergeCartAsync(string? sub)
    {
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId))
            return new ServiceResponse<ShoppingCartDto>
            {
                Status = 400,
                Message = "Thông tin người dùng không hợp lệ"
            };
        throw new NotImplementedException();
    }
}