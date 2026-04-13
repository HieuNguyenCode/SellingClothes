namespace Services.Dto.Requests;

public class ShoppingCartItemUpdateDto
{
    public bool IsCombo { get; set; }

    public string Name { get; set; }

    public string? Size { get; set; }

    public string? Color { get; set; }

    public int? Quantity { get; set; }

    public List<CartComboProductUpdateDto> Products { get; set; }
}