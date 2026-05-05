using Services.Dto.Requests;
using Services.Dto.Responses;

namespace Services.Interfaces;

public interface IPaymentService
{
    Task<ServiceResponse> AddProductToCartAsync(ShoppingCartUpdateDto item, string? sub);

    Task<ServiceResponse<PagedResult<OrderAdminResponseDto>>> GetAllOrdersForAdminAsync(int page = 1,
        int pageSize = 10, string? orderStatus = null);

    Task<ServiceResponse<PagedResult<OrderAdminResponseDto>>> GetUserOrdersAsync(string? sub, string? sessionId,
        int page = 1, int pageSize = 10);

    Task<ServiceResponse> UpdateOrderStatusAsync(Guid orderId, OrderStatusUpdateDto updateDto, string? updateBy);
}