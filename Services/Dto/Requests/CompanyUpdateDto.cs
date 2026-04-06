using System.ComponentModel.DataAnnotations;

namespace Services.Dto.Requests;

public class CompanyUpdateDto
{
    [Required(ErrorMessage = "Tên công ty là bắt buộc")]
    public string Name { get; set; } = null!;
}
