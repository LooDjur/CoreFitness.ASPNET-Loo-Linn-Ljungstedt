using Application.Sessions.Output;

namespace Presentation.WebApp.Models.Sessions;

public class SessionsViewModel
{
    public IEnumerable<SessionResponse> Sessions { get; set; } = [];
    public SessionFormViewModel Form { get; set; } = new();
    public bool IsMember { get; set; }
}
