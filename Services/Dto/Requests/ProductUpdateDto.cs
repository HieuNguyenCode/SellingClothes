using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Services.Dto.Requests;

public class ProductUpdateDto
{
    [Required(ErrorMessage = "Tên sản phẩm không được để trống.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Mô tả sản phẩm không được để trống.")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Giá sản phẩm không được để trống.")]
    public int Price { get; set; }

    [Required(ErrorMessage = "Ảnh chính sản phẩm không được để trống.")]
    public IFormFile Image { get; set; }

    public List<IFormFile>? Images { get; set; }

    [Required(ErrorMessage = "Loại sản phẩm không được để trống.")]
    public string TypeName { get; set; } = null!;

    [Required(ErrorMessage = "Thương hiệu sản phẩm không được để trống.")]
    public string CompanyName { get; set; } = null!;
}
