using Application.Abstractions.Authentication;
using Application.Commo;
using Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identit.Services;

public sealed class IdentityAuthService(
    UserManager<AppUser> userManager,
    RoleManager<AppRole> roleManager,
    SignInManager<AppUser> signInManager) : IAuthService
{
    public async Task<Result<Guid>> RegisterIdentityAsync(string email, string password, string role, CancellationToken ct)
    {
        var appUser = AppUser.Create(email);
        var result = await userManager.CreateAsync(appUser, password);

        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            return Result.Failure<Guid>(Error.Validation(error.Code, error.Description));
        }

        if (!await roleManager.RoleExistsAsync(role))
        {
            var roleCreateResult = await roleManager.CreateAsync(new AppRole { Name = role });
            if (!roleCreateResult.Succeeded)
            {
                return Result.Failure<Guid>(Error.Validation("Role.Error", "Kunde inte skapa rollen."));
            }
        }

        var roleResult = await userManager.AddToRoleAsync(appUser, role);

        if (!roleResult.Succeeded)
        {
            var error = roleResult.Errors.First();
            return Result.Failure<Guid>(Error.Validation(error.Code, error.Description));
        }

        return Result.Success(Guid.Parse(appUser.Id));
    }

    public async Task<AuthResult> SignInUserAsync(string email, string password, bool rememberMe)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return AuthResult.Failed(DomainErrors.Authentication.InvalidCredentials);

        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return AuthResult.Failed(DomainErrors.Authentication.InvalidCredentials);

        var result = await signInManager.PasswordSignInAsync(user, password, rememberMe, false);

        if (result.Succeeded) return AuthResult.Ok();

        return AuthResult.Failed(result.IsLockedOut
            ? DomainErrors.Authentication.LockedOut
            : DomainErrors.Authentication.InvalidCredentials);
    }

    public Task SignOutUserAsync() => signInManager.SignOutAsync();
}