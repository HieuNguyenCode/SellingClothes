namespace Services.Dto.Requests;

public class CartComboProductUpdateDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Image { get; set; }

    public string Size { get; set; }

    public string Color { get; set; }

    public int Quantity { get; set; }
}
