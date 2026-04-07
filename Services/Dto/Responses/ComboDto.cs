namespace Services.Dto.Responses;

public class ComboDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public int Price { get; set; }

    public int? PriceSale { get; set; }

    public string Image { get; set; } = null!;

    public List<ComboProductDto> Products { get; set; } = [];
}