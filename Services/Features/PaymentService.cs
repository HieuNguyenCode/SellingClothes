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
        if (sub == null && item.Session == null)
            return new ServiceResponse
            {
                Status = 400,
                Message = "Vui lòng đăng nhập hoặc cung cấp session để thêm sản phẩm vào giỏ hàng."
            };
        Guid.TryParse(sub, out var userId);
        await using var transaction = await appDbContext.Database.BeginTransactionAsync();
        try
        {
            var order = new Order
            {
                Idorder = Guid.NewGuid(),
                Iduser = sub == null ? null : userId,
                SessionId = sub == null ? item.Session : null,
                CustomerName = item.CustomerName,
                PhoneNumber = item.PhoneNumber,
                ShippingAddress = item.ShippingAddress,
                PaymentMethod = item.PaymentMethod,
                OrderStatus = "Pending",
                PaymentStatus = "Unpaid"
            };


            var totalPrice = 0;
            var orderDetails = new List<OrderDetail>();

            foreach (var shoppingCart in item.ShoppingCart)
            {
                if (shoppingCart.IsCombo)
                {
                    var combo = await appDbContext.Combos.FirstOrDefaultAsync(c => c.Name == shoppingCart.Name);
                    if (combo == null)
                    {
                        await transaction.RollbackAsync();
                        return new ServiceResponse
                        {
                            Status = 404,
                            Message = $"Combo '{shoppingCart.Name}' không tồn tại."
                        };
                    }

                    var sale = await appDbContext.SaleCombos.Where(s => s.Idcombo == combo.Idcombo &&
                                                                        s.StartDate <= DateTime.Now &&
                                                                        s.EndDate >= DateTime.Now).ToListAsync();
                    if (sale.Count != 0)
                        totalPrice += sale.MinBy(s => s.Price)!.Price * shoppingCart.Quantity;
                    else
                        totalPrice += combo.Price * shoppingCart.Quantity;
                    orderDetails.Add(new OrderDetail
                    {
                        Idorder = order.Idorder,
                        Idcombo = combo.Idcombo,
                        Quantity = shoppingCart.Quantity
                    });
                    continue;
                }

                var product = await appDbContext.Products
                    .Include(p => p.Sizes)
                    .Include(product => product.Colors)
                    .FirstOrDefaultAsync(p => p.Name == shoppingCart.Name &&
                                              p.IsPublished &&
                                              !p.IsDeleted);
                if (product == null)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse
                    {
                        Status = 404,
                        Message = $"Sản phẩm '{shoppingCart.Name}' không tồn tại."
                    };
                }

                var sales = await appDbContext.SaleProducts.Where(s => s.Idproduct == product.Idproduct &&
                                                                       s.StartDate <= DateTime.Now &&
                                                                       s.EndDate >= DateTime.Now).ToListAsync();
                if (sales.Count != 0)
                    totalPrice += sales.MinBy(s => s.Price)!.Price * shoppingCart.Quantity;
                else
                    totalPrice += product.Price * shoppingCart.Quantity;
                orderDetails.Add(new OrderDetail
                {
                    Idorder = order.Idorder,
                    Idproduct = product.Idproduct,
                    Quantity = shoppingCart.Quantity,
                    Idsize = product.Sizes.FirstOrDefault(s => s.Name == shoppingCart.Size)?.Idsize,
                    Idcolor = product.Colors.FirstOrDefault(c => c.Name == shoppingCart.Color)?.Idcolor
                });
            }

            order.TotalPrice = totalPrice;
            await appDbContext.Orders.AddAsync(order);
            await appDbContext.OrderDetails.AddRangeAsync(orderDetails);
            await appDbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ServiceResponse
            {
                Status = 200,
                Message = "Sản phẩm đã được thêm vào giỏ hàng thành công."
            };
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            logger.LogError(e, "Lỗi khi thêm sản phẩm vào giỏ hàng.");
            return new ServiceResponse
            {
                Status = 500,
                Message = "Có lỗi xảy ra trong quá trình lưu dữ liệu."
            };
        }
    }
}