using Domain.Sessions;
using System.ComponentModel.DataAnnotations;

namespace Presentation.WebApp.Models.Sessions;

public class SessionFormViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Titeln får inte vara tom")]
    [Display(Name = "Passets Titel", Prompt = "t.ex. Morgonyoga")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Välj en instruktör")]
    [Display(Name = "Instruktör", Prompt = "Vem håller i passet?")]
    public string Instructor { get; set; } = string.Empty;

    [Required(ErrorMessage = "Välj en kategori")]
    [Display(Name = "Kategori")]
    public SessionCategory Category { get; set; }

    [Required(ErrorMessage = "Ange starttid")]
    [Display(Name = "Starttid och datum")]
    [DataType(DataType.DateTime)]

    [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
    public DateTime StartTime { get; set; }

    [Required(ErrorMessage = "Ange max antal platser")]
    [Range(1, 100, ErrorMessage = "Antal platser måste vara mellan 1 och 100")]
    [Display(Name = "Max antal platser")]
    public int MaxCapacity { get; set; } = 10;

    [Required(ErrorMessage = "Beskriv passet")]
    [Display(Name = "Beskrivning", Prompt = "Berätta lite om passet...")]
    [DataType(DataType.MultilineText)]
    public string Description { get; set; } = string.Empty;

    public bool IsEdit => Id.HasValue;
}