using System.ComponentModel.DataAnnotations;

namespace Services.Dto.Requests;

public class RegisterDto
{
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = null!;
}
