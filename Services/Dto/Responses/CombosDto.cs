namespace Services.Dto.Responses;

public class CombosDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public int Price { get; set; }

    public int? PriceSale { get; set; }

    public string Image { get; set; } = null!;
}