using System.ComponentModel.DataAnnotations;

namespace Presentation.WebApp.Models.CustomerService;

public class ContactViewModel
{
    [Required(ErrorMessage = "You must enter a first name")]
    [Display(Name = "First Name", Prompt = "")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "You must enter a last name")]
    [Display(Name = "Last Name", Prompt = "")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "You must enter an email address")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [Display(Name = "Email Address", Prompt = "name@example.com")]
    public string Email { get; set; } = null!;

    [Phone(ErrorMessage = "You must enter a valid phone number")]
    [Display(Name = "Phone Number", Prompt = "format: 070-123 45 67")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "You must enter a message")]
    [Display(Name = "Message", Prompt = "Message")]
    public string Message { get; set; } = null!;

    [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the terms and conditions")]
    [Display(Name = "I accept t´hat CoreFitness will save my information.")]
    public bool AcceptSavePersonalInformation { get; set; }
}
