namespace Services.Dto.Responses;

public class ShoppingCartItemDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Size { get; set; }

    public string Color { get; set; }

    public string Image { get; set; }

    public IEnumerable<CartComboProductDto> Products { get; set; }

    public int Quantity { get; set; }

    public int Price { get; set; }
}
