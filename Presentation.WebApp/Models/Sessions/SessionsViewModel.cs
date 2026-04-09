using Application.Sessions.Output;

namespace Presentation.WebApp.Models.Sessions;

public class SessionsViewModel
{
    public IEnumerable<SessionOutput> Sessions { get; set; } = [];
    public SessionFormViewModel Form { get; set; } = new();
}
