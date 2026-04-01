using Application.Commo;
using Domain.Common;

namespace Application.Abstractions.Authentication;

public interface IAuthService
{
    Task<Result<Guid>> RegisterIdentityAsync(string email, string password, string role, CancellationToken ct = default);
    Task<AuthResult> SignInUserAsync(string email, string password, bool rememberMe);
    Task SignOutUserAsync();
}
