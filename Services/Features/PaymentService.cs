using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Dto.Requests;
using Services.Dto.Responses;
using Services.Interfaces;

namespace Services.Features;

public class PaymentService(
    AppDbContext appDbContext,
    ILogger<PaymentService> logger) : IPaymentService
{
    public async Task<ServiceResponse> AddProductToCartAsync(ShoppingCartUpdateDto item, string? sub)
    {
        // Cập nhật logic kiểm tra session và user hợp lý hơn
        if (string.IsNullOrEmpty(sub) && string.IsNullOrEmpty(item.Session))
            return new ServiceResponse
            {
                Status = 400,
                Message = "Vui lòng đăng nhập hoặc cung cấp session để thực hiện."
            };

        Guid? userId = null;
        if (Guid.TryParse(sub, out var parsedUserId)) userId = parsedUserId;

        await using var transaction = await appDbContext.Database.BeginTransactionAsync();
        try
        {
            var orderId = Guid.NewGuid();
            var order = new Order
            {
                Idorder = orderId,
                Iduser = userId,
                SessionId = userId == null ? item.Session : null,
                CustomerName = item.CustomerName,
                PhoneNumber = item.PhoneNumber,
                ShippingAddress = item.ShippingAddress,
                PaymentMethod = item.PaymentMethod,
                OrderStatus = "Pending",
                PaymentStatus = "Unpaid"
            };

            var totalPrice = 0;
            var orderDetails = new List<OrderDetail>();
            var orderDetailProducts =
                new List<OrderDetailProduct>(); // Bảng mới lưu chi tiết sản phẩm của Combo theo schema

            foreach (var cartItem in item.ShoppingCart)
            {
                var orderDetailId = Guid.NewGuid();

                if (cartItem.IsCombo)
                {
                    // Xử lý Combo
                    var combo = await appDbContext.Combos.FirstOrDefaultAsync(c =>
                        c.Name == cartItem.Name && c.IsPublished && !c.IsDeleted);
                    if (combo == null)
                    {
                        await transaction.RollbackAsync();
                        return new ServiceResponse
                        {
                            Status = 404,
                            Message = $"Combo '{cartItem.Name}' không tồn tại hoặc đã bị ẩn."
                        };
                    }

                    // Kiểm tra EndDate có thể null trong logic Sale
                    var sales = await appDbContext.SaleCombos
                        .Where(s => s.Idcombo == combo.Idcombo && s.StartDate <= DateTime.Now &&
                                    (s.EndDate == null || s.EndDate >= DateTime.Now))
                        .ToListAsync();

                    var unitPrice = sales.Count != 0 ? sales.Min(s => s.Price) : combo.Price;
                    totalPrice += unitPrice * cartItem.Quantity;

                    orderDetails.Add(new OrderDetail
                    {
                        IdorderDetail = orderDetailId,
                        Idorder = orderId,
                        Idcombo = combo.Idcombo,
                        Quantity = cartItem.Quantity,
                        UnitPrice = unitPrice // Cập nhật UnitPrice bắt buộc theo database
                    });

                    // Lưu các sản phẩm thuộc Combo vào bảng OrderDetailProduct
                    foreach (var comboProduct in cartItem.Products)
                    {
                        var product = await appDbContext.Products
                            .Include(p => p.Sizes)
                            .Include(p => p.Colors)
                            .FirstOrDefaultAsync(p => p.Name == comboProduct.Name && p.IsPublished && !p.IsDeleted);

                        if (product == null)
                        {
                            await transaction.RollbackAsync();
                            return new ServiceResponse
                            {
                                Status = 400,
                                Message = $"Sản phẩm '{comboProduct.Name}' trong combo không tồn tại."
                            };
                        }

                        orderDetailProducts.Add(new OrderDetailProduct
                        {
                            IdorderDetailProduct = Guid.NewGuid(),
                            IdorderDetail = orderDetailId,
                            Idproduct = product.Idproduct,
                            Quantity = comboProduct.Quantity > 0
                                ? comboProduct.Quantity
                                : 1, // Số lượng trong combo
                            Idsize = product.Sizes.FirstOrDefault(s => s.Name == comboProduct.Size)?.Idsize,
                            Idcolor = product.Colors.FirstOrDefault(c => c.Name == comboProduct.Color)?.Idcolor
                        });
                    }

                    continue;
                }

                // Xử lý Product đơn lẻ
                var singleProduct = await appDbContext.Products
                    .Include(p => p.Sizes)
                    .Include(p => p.Colors)
                    .FirstOrDefaultAsync(p => p.Name == cartItem.Name && p.IsPublished && !p.IsDeleted);

                if (singleProduct == null)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse
                    {
                        Status = 404,
                        Message = $"Sản phẩm '{cartItem.Name}' không tồn tại hoặc đã bị ẩn."
                    };
                }

                var singleSales = await appDbContext.SaleProducts
                    .Where(s => s.Idproduct == singleProduct.Idproduct && s.StartDate <= DateTime.Now &&
                                (s.EndDate == null || s.EndDate >= DateTime.Now))
                    .ToListAsync();

                var singleUnitPrice = singleSales.Count != 0 ? singleSales.Min(s => s.Price) : singleProduct.Price;
                totalPrice += singleUnitPrice * cartItem.Quantity;

                orderDetails.Add(new OrderDetail
                {
                    IdorderDetail = orderDetailId,
                    Idorder = orderId,
                    Idproduct = singleProduct.Idproduct,
                    Quantity = cartItem.Quantity,
                    UnitPrice = singleUnitPrice, // Cập nhật UnitPrice
                    Idsize = singleProduct.Sizes.FirstOrDefault(s => s.Name == cartItem.Size)?.Idsize,
                    Idcolor = singleProduct.Colors.FirstOrDefault(c => c.Name == cartItem.Color)?.Idcolor
                });
            }

            order.TotalPrice = totalPrice;

            await appDbContext.Orders.AddAsync(order);
            await appDbContext.OrderDetails.AddRangeAsync(orderDetails);

            if (orderDetailProducts.Any()) await appDbContext.OrderDetailProducts.AddRangeAsync(orderDetailProducts);

            await appDbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ServiceResponse
            {
                Status = 200,
                Message = "Tạo đơn hàng thành công." // Cập nhật message cho đúng nghiệp vụ
            };
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            logger.LogError(e, "Lỗi khi xử lý đơn hàng.");
            return new ServiceResponse
            {
                Status = 500,
                Message = "Có lỗi xảy ra trong quá trình lưu dữ liệu."
            };
        }
    }

    public async Task<ServiceResponse<PagedResult<OrderAdminResponseDto>>> GetAllOrdersForAdminAsync(int page = 1,
        int pageSize = 10, string? orderStatus = null)
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var query = appDbContext.Orders.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(orderStatus)) query = query.Where(o => o.OrderStatus == orderStatus);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var orders = await query
                .OrderByDescending(o => o.CreateAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderAdminResponseDto
                {
                    Idorder = o.Idorder,
                    Iduser = o.Iduser,
                    CustomerName = o.CustomerName,
                    PhoneNumber = o.PhoneNumber,
                    ShippingAddress = o.ShippingAddress,
                    TotalPrice = o.TotalPrice,
                    OrderStatus = o.OrderStatus ?? "Pending",
                    PaymentMethod = o.PaymentMethod ?? "COD",
                    PaymentStatus = o.PaymentStatus ?? "Unpaid",
                    CreateAt = o.CreateAt ?? DateTime.MinValue,
                    Items = o.OrderDetails.Select(od => new OrderDetailAdminDto
                    {
                        ItemName = od.Idproduct != null
                            ? od.IdproductNavigation != null ? od.IdproductNavigation.Name : "N/A"
                            : od.IdcomboNavigation != null
                                ? od.IdcomboNavigation.Name
                                : "N/A",
                        ItemType = od.Idproduct != null ? "Product" : "Combo",
                        SizeName = od.IdsizeNavigation != null ? od.IdsizeNavigation.Name : null,
                        ColorName = od.IdcolorNavigation != null ? od.IdcolorNavigation.Name : null,
                        Quantity = od.Quantity,
                        UnitPrice = od.UnitPrice,
                        SubTotal = od.SubTotal ?? (od.Quantity * od.UnitPrice),
                        ComboItems = od.Idcombo != null 
                            ? od.OrderDetailProducts.Select(odp => new OrderDetailComboItemDto
                            {
                                ProductName = odp.IdproductNavigation != null ? odp.IdproductNavigation.Name : "N/A",
                                SizeName = odp.IdsizeNavigation != null ? odp.IdsizeNavigation.Name : null,
                                ColorName = odp.IdcolorNavigation != null ? odp.IdcolorNavigation.Name : null,
                                Quantity = odp.Quantity
                            }).ToList()
                            : null
                        }).ToList()
                        })
                .ToListAsync();

            return new ServiceResponse<PagedResult<OrderAdminResponseDto>>
            {
                Status = 200,
                Message = "Lấy danh sách đơn hàng thành công.",
                Data = new PagedResult<OrderAdminResponseDto>
                {
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    CurrentPage = page,
                    Data = orders
                }
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Lỗi khi admin lấy danh sách đơn hàng.");
            return new ServiceResponse<PagedResult<OrderAdminResponseDto>>
            {
                Status = 500,
                Message = "Đã xảy ra lỗi khi lấy danh sách đơn hàng."
            };
        }
    }

    public async Task<ServiceResponse<PagedResult<OrderAdminResponseDto>>> GetUserOrdersAsync(string? sub,
        string? sessionId, int page = 1, int pageSize = 10)
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var query = appDbContext.Orders.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(sub) && Guid.TryParse(sub, out var userId))
            {
                query = query.Where(o => o.Iduser == userId);
            }
            else if (!string.IsNullOrEmpty(sessionId))
            {
                query = query.Where(o => o.SessionId == sessionId);
            }
            else
            {
                return new ServiceResponse<PagedResult<OrderAdminResponseDto>>
                {
                    Status = 400,
                    Message = "Thiếu thông tin người dùng hoặc SessionId."
                };
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var orders = await query
                .OrderByDescending(o => o.CreateAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderAdminResponseDto
                {
                    Idorder = o.Idorder,
                    Iduser = o.Iduser,
                    CustomerName = o.CustomerName,
                    PhoneNumber = o.PhoneNumber,
                    ShippingAddress = o.ShippingAddress,
                    TotalPrice = o.TotalPrice,
                    OrderStatus = o.OrderStatus ?? "Pending",
                    PaymentMethod = o.PaymentMethod ?? "COD",
                    PaymentStatus = o.PaymentStatus ?? "Unpaid",
                    CreateAt = o.CreateAt ?? DateTime.MinValue,
                    Items = o.OrderDetails.Select(od => new OrderDetailAdminDto
                    {
                        ItemName = od.Idproduct != null
                            ? (od.IdproductNavigation != null ? od.IdproductNavigation.Name : "N/A")
                            : (od.IdcomboNavigation != null ? od.IdcomboNavigation.Name : "N/A"),
                        ItemType = od.Idproduct != null ? "Product" : "Combo",
                        SizeName = od.IdsizeNavigation != null ? od.IdsizeNavigation.Name : null,
                        ColorName = od.IdcolorNavigation != null ? od.IdcolorNavigation.Name : null,
                        Quantity = od.Quantity,
                        UnitPrice = od.UnitPrice,
                        SubTotal = od.SubTotal ?? (od.Quantity * od.UnitPrice),
                        ComboItems = od.Idcombo != null
                            ? od.OrderDetailProducts.Select(odp => new OrderDetailComboItemDto
                            {
                                ProductName = odp.IdproductNavigation != null ? odp.IdproductNavigation.Name : "N/A",
                                SizeName = odp.IdsizeNavigation != null ? odp.IdsizeNavigation.Name : null,
                                ColorName = odp.IdcolorNavigation != null ? odp.IdcolorNavigation.Name : null,
                                Quantity = odp.Quantity
                            }).ToList()
                            : null
                    }).ToList()
                })
                .ToListAsync();

            return new ServiceResponse<PagedResult<OrderAdminResponseDto>>
            {
                Status = 200,
                Message = "Lấy lịch sử đơn hàng thành công.",
                Data = new PagedResult<OrderAdminResponseDto>
                {
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    CurrentPage = page,
                    Data = orders
                }
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Lỗi khi lấy lịch sử đơn hàng.");
            return new ServiceResponse<PagedResult<OrderAdminResponseDto>>
            {
                Status = 500,
                Message = "Đã xảy ra lỗi khi lấy lịch sử đơn hàng."
            };
        }
    }

    public async Task<ServiceResponse> UpdateOrderStatusAsync(Guid orderId, OrderStatusUpdateDto updateDto,
        string? updateBy)
    {
        try
        {
            var order = await appDbContext.Orders.FirstOrDefaultAsync(o => o.Idorder == orderId);

            if (order == null)
            {
                return new ServiceResponse
                {
                    Status = 404,
                    Message = "Không tìm thấy đơn hàng."
                };
            }

            if (!string.IsNullOrEmpty(updateDto.OrderStatus))
            {
                order.OrderStatus = updateDto.OrderStatus;
            }

            if (!string.IsNullOrEmpty(updateDto.PaymentStatus))
            {
                order.PaymentStatus = updateDto.PaymentStatus;
            }

            if (Guid.TryParse(updateBy, out var updateById))
            {
                order.UpdateBy = updateById;
            }

            order.UpdateAt = DateTime.Now;

            await appDbContext.SaveChangesAsync();

            return new ServiceResponse
            {
                Status = 200,
                Message = "Cập nhật trạng thái đơn hàng thành công."
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Lỗi khi cập nhật trạng thái đơn hàng {OrderId}", orderId);
            return new ServiceResponse
            {
                Status = 500,
                Message = "Đã xảy ra lỗi khi cập nhật trạng thái đơn hàng."
            };
        }
    }
}