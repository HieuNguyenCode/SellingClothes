namespace Services.Dto.Responses;

public class SaleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public List<SaleProductsDto> SaleProducts { get; set; } = [];
    public List<SaleCombosDto> SaleCombos { get; set; } = [];
}