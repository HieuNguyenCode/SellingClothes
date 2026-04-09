namespace Services.Dto.Requests;

public class SaleUpdateDto
{
    public string Name { get; set; }
    public List<SaleProductUpdateDto> SaleProducts { get; set; } = [];
    public List<SaleComboUpdateDto> SaleCombos { get; set; } = [];
}