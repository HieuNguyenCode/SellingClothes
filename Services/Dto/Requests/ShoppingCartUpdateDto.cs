namespace Services.Dto.Requests;

public class ShoppingCartUpdateDto
{
    public string CustomerName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string ShippingAddress { get; set; } = null!;

    public string PaymentMethod { get; set; } = "COD";

    public string? Session { get; set; }

    public List<ShoppingCartItemUpdateDto> ShoppingCart { get; set; }
}