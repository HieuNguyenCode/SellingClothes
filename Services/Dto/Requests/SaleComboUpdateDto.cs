namespace Services.Dto.Requests;

public class SaleComboUpdateDto
{
    public string Name { get; set; }
    public int Price { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
