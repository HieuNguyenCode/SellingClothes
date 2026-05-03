namespace Services.Dto.Responses;

public class ComboProductDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    
    public string Image { get; set; } = null!;
    
    public List<string> Size { get; set; } = [];
    
    public List<string> Color { get; set; } = [];

    public int Quantity { get; set; }
}