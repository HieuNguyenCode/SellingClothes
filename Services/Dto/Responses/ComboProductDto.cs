namespace Services.Dto.Responses;

public class ComboProductDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public int Quantity { get; set; }
}