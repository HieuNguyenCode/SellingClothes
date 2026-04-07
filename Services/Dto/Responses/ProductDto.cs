namespace Services.Dto.Responses;

public class ProductDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public int Price { get; set; }

    public int? PriceSale { get; set; }
    
    public string Image { get; set; } = null!;

    public List<string> Images { get; set; } = [];

    public List<string> Sizes { get; set; } = [];

    public List<string> Colors { get; set; } = [];

    public string TypeName { get; set; } = null!;

    public string CompanyName { get; set; } = null!;

    public string? Description { get; set; } = null!;
}
