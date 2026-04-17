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

    public async Task<Result<Guid>> RegisterExternalIdentityAsync(string email, string provider, string providerKey, CancellationToken ct)
    {
        var appUser = await userManager.FindByEmailAsync(email);

        if (appUser is null)
        {
            appUser = AppUser.Create(email);
            appUser.EmailConfirmed = true;

            var createResult = await userManager.CreateAsync(appUser);
            if (!createResult.Succeeded)
            {
                return Result.Failure<Guid>(Error.Validation("Auth.Error", createResult.Errors.First().Description));
            }
        }

        var logins = await userManager.GetLoginsAsync(appUser);
        if (!logins.Any(l => l.LoginProvider == provider))
        {
            var addLoginResult = await userManager.AddLoginAsync(appUser, new UserLoginInfo(provider, providerKey, provider));

            if (!addLoginResult.Succeeded)
                return Result.Failure<Guid>(DomainErrors.Authentication.ExternalLoginFailed);
        }

        return Result.Success(Guid.Parse(appUser.Id));
    }

    public async Task<Result> UpdateIdentityUserAsync(Guid userId, string newEmail, string firstName, string lastName, string? phoneNumber, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Result.Failure(DomainErrors.User.NotFound);
        }
        user.Email = newEmail;
        user.UserName = newEmail;
        user.FirstName = firstName;
        user.LastName = lastName;
        user.PhoneNumber = phoneNumber;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            return Result.Failure(Error.Validation(error.Code, error.Description));
        }

        await signInManager.RefreshSignInAsync(user);
        return Result.Success();
    }

    public async Task<Result> DeleteIdentityUserAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Result.Failure(DomainErrors.User.NotFound);
        }

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            return Result.Failure(Error.Validation(error.Code, error.Description));
        }

        return Result.Success();
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
    public async Task<bool> UserExistsAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        return user != null;
    }
    public Task SignOutUserAsync() => signInManager.SignOutAsync();
}