namespace Services.Dto.Responses;

public class CartComboProductDto
{
    public Guid Id { get; set; }
    public Guid IdProduct { get; set; }
    public string Name { get; set; }
    public string Size { get; set; }
    public Guid? SizeId { get; set; }
    public string Color { get; set; }
    public Guid? ColorId { get; set; }
    public string Image { get; set; }
    public int Quantity { get; set; }
}
