using Application.CustomerSupport.Input;

namespace Application.CustomerSupport;

public interface IContactFormService
{
    Task RegisterContactRequestAsync(RegisterContactRequestInput request, CancellationToken ct = default);
}