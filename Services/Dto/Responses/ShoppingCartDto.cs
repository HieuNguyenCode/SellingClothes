namespace Services.Dto.Responses;

public class ShoppingCartDto
{
    public int TotalPrice { get; set; }

    public List<ShoppingCartItemDto> ShoppingCartItems { get; set; }
}
