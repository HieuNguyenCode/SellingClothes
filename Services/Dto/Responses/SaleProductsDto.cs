namespace Services.Dto.Responses;

public class SaleProductsDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Image { get; set; }

    public int Price { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}
