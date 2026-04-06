using System.ComponentModel.DataAnnotations;

namespace Services.Dto.Requests;

public class LoginDto
{
    [Required(ErrorMessage = "Tài khoản là bắt buộc")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    public string Password { get; set; } = null!;
}
