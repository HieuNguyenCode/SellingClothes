using Microsoft.AspNetCore.Http;

namespace Services.Dto.Requests;

public class ComboUpdateDto
{
    public string Name { get; set; } = null!;
    public int Price { get; set; }
    public IFormFile Image { get; set; } = null!;
    public List<ComboProductUpdateDto> ListProducts { get; set; } = [];
}