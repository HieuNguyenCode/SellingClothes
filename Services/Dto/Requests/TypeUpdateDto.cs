using System.ComponentModel.DataAnnotations;

namespace Services.Dto.Requests;

public class TypeUpdateDto
{
    [Required(ErrorMessage = "Tên loại tàu là bắt buộc")]
    public string Name { get; set; } = null!;
}
