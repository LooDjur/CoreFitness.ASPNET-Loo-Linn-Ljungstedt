using System.ComponentModel.DataAnnotations;

namespace Presentation.WebApp.Models.Authentication;

public class SignUpFormViewModel
{
    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "Email address must be valid")]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email Address", Prompt = "username@example.com")]
    public string Email { get; set; } = null!;

    [Display(Name = "I accept the user terms & conditions")]
    [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept user terms & conditions.")]
    public bool TermsAndConditions { get; set; }

    public string? ErrorMessage { get; set; }
}
