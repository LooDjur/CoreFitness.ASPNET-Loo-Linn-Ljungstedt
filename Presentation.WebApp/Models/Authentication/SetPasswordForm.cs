using System.ComponentModel.DataAnnotations;

namespace Presentation.WebApp.Models.Authentication;

public class SetPasswordForm
{
    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, ErrorMessage = "The password must be at least 4 characters long.", MinimumLength = 4)]
    [DataType(DataType.Password)]
    [Display(Name = "Password", Prompt = "Enter Password")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Password must be confirmed")]
    [Compare(nameof(Password), ErrorMessage = "Password must match")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password", Prompt = "Confirm Password")]
    public string ConfirmPassword { get; set; } = null!;

    public string? ErrorMessage { get; set; }
}
