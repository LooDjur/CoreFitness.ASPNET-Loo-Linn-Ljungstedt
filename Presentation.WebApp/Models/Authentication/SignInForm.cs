using System.ComponentModel.DataAnnotations;

namespace Presentation.WebApp.Models.Authentication;

public class SignInForm
{
    [Required(ErrorMessage = "Email is required")]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email", Prompt = "username@example.com")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Password", Prompt = "Enter Password")]
    public string Password { get; set; } = null!;

    [Display(Name = "Remember Me")]
    public bool RememberMe { get; set; }

    public string? ErrorMessage { get; set; }
}
