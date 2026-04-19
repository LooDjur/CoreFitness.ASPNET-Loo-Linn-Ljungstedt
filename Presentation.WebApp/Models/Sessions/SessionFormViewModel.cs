using Domain.Sessions;
using System.ComponentModel.DataAnnotations;

namespace Presentation.WebApp.Models.Sessions;

public class SessionFormViewModel : IValidatableObject
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Title cannot be empty")]
    [Display(Name = "Session Title", Prompt = "e.g. Morning Yoga")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select an instructor")]
    [Display(Name = "Instructor", Prompt = "Who is leading the session?")]
    public string Instructor { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select a category")]
    [Display(Name = "Category")]
    public SessionCategory? Category { get; set; }

    [Required(ErrorMessage = "Please provide a start time")]
    [Display(Name = "Start Time and Date")]
    [DataType(DataType.DateTime)]
    [FutureDate(ErrorMessage = "Sessions must be scheduled at least one day in advance.")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
    public DateTime? StartTime { get; set; } = DateTime.Today.AddDays(1).AddHours(10);

    [Required(ErrorMessage = "Please provide an end time")]
    [Display(Name = "End Time")]
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
    public DateTime? EndTime { get; set; } = DateTime.Today.AddDays(1).AddHours(11);

    [Required(ErrorMessage = "Please specify maximum capacity")]
    [Range(10, 20, ErrorMessage = "Capacity must be between 10 and 20")]
    [Display(Name = "Max Capacity")]
    public int MaxCapacity { get; set; } = 10;

    [Required(ErrorMessage = "Please provide a description")]
    [Display(Name = "Description", Prompt = "Tell us a bit about the session...")]
    [DataType(DataType.MultilineText)]
    public string Description { get; set; } = string.Empty;

    public bool IsEdit => Id.HasValue;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartTime.HasValue && EndTime.HasValue)
        {
            if (EndTime.Value <= StartTime.Value)
            {
                yield return new ValidationResult(
                    "End time must be after start time.",
                    [nameof(EndTime)]);
            }

            if (EndTime.Value > StartTime.Value.AddHours(4))
            {
                yield return new ValidationResult(
                    "A session cannot be longer than 4 hours.",
                    [nameof(EndTime)]);
            }
        }
    }
}

public class FutureDateAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime dateTime)
        {
            if (dateTime.Date <= DateTime.Today)
            {
                return new ValidationResult(ErrorMessage ?? "Date must be in the future.");
            }
        }

        return ValidationResult.Success;
    }
}