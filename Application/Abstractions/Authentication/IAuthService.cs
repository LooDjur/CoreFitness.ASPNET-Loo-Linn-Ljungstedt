using Application.Commo;
using Domain.Common;

namespace Application.Abstractions.Authentication;

public interface IAuthService
{
    Task<Result<Guid>> RegisterIdentityAsync(string email, string password, string role, CancellationToken ct = default);
    Task<Result<Guid>> RegisterExternalIdentityAsync(string email, string provider, string providerKey, CancellationToken ct = default);
    Task<Result> UpdateIdentityUserAsync(Guid userId, string newEmail, string firstName, string lastName, string? phoneNumber, CancellationToken ct = default);
    Task<Result> DeleteIdentityUserAsync(Guid userId, CancellationToken ct = default);
    Task<AuthResult> SignInUserAsync(string email, string password, bool rememberMe);
    Task<bool> UserExistsAsync(string email);
    Task SignOutUserAsync();
}
