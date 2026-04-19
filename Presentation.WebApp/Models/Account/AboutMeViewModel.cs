using System.ComponentModel.DataAnnotations;

namespace Presentation.WebApp.Models.Account;
public class AboutMeViewModel
{
    [Display(Name = "First Name", Prompt = "Enter your first name")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "First name is required")]
    public string? FirstName { get; set; }

    [Display(Name = "Last Name", Prompt = "Enter your last name")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Last name is required")]
    public string? LastName { get; set; }

    [Display(Name = "Email Address", Prompt = "username@example.com")]
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string? Email { get; set; }

    [Display(Name = "Phone Number", Prompt = "ex. 070-123 45 67")]
    [DataType(DataType.PhoneNumber)]
    [Phone(ErrorMessage = "Invalid phone number")]
    public string? Phone { get; set; }

    [Display(Name = "Profile Image")]
    [DataType(DataType.Upload)]
    public IFormFile? ProfileImage { get; set; }

    public string? ProfileImageUrl { get; set; }
}

