using FluentAssertions;
using Tests.Common;
using Microsoft.AspNetCore.Identity;

namespace Tests.Identity;

public class IdentityAuthServiceTests : BaseIdentityIntegrationTest
{
    [Fact]
    public async Task RegisterIdentityAsync_Should_CreateUserAndAssignRole_WhenDataIsValid()
    {
        // Arrange
        var email = "test@fitness.se";
        var password = "Password123!";
        var role = "Member";

        // Act
        var result = await AuthService.RegisterIdentityAsync(email, password, role, default);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var user = await UserManager.FindByEmailAsync(email);
        user.Should().NotBeNull();
        user!.Email.Should().Be(email);

        var roles = await UserManager.GetRolesAsync(user);
        roles.Should().Contain(role);
    }

    [Fact]
    public async Task RegisterExternalIdentityAsync_Should_CreateUserWithExternalLogin_WhenNewUser()
    {
        // Arrange
        var email = "github-user@test.se";
        var provider = "GitHub";
        var providerKey = "gh_123456";

        // Act
        var result = await AuthService.RegisterExternalIdentityAsync(email, provider, providerKey, default);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var user = await UserManager.FindByEmailAsync(email);
        user.Should().NotBeNull();
        user!.EmailConfirmed.Should().BeTrue();

        var logins = await UserManager.GetLoginsAsync(user);
        logins.Should().ContainSingle(l => l.LoginProvider == provider && l.ProviderKey == providerKey);
    }

    [Fact]
    public async Task UserExistsAsync_Should_ReturnTrue_WhenUserExists()
    {
        // Arrange
        var email = "exists@test.se";
        await AuthService.RegisterIdentityAsync(email, "Pass123!", "Member");

        // Act
        var exists = await AuthService.UserExistsAsync(email);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateIdentityUserAsync_Should_UpdateAllFields()
    {
        // Arrange
        var email = "old@test.se";
        var registration = await AuthService.RegisterIdentityAsync(email, "Pass123!", "Member");
        var userId = registration.Value;

        var newEmail = "new@test.se";
        var firstName = "Tränings";
        var lastName = "Anders";

        // Act
        var result = await AuthService.UpdateIdentityUserAsync(userId, newEmail, firstName, lastName, "112");

        // Assert
        result.IsSuccess.Should().BeTrue();

        var user = await UserManager.FindByIdAsync(userId.ToString());
        user!.Email.Should().Be(newEmail);
        user.FirstName.Should().Be(firstName);
        user.LastName.Should().Be(lastName);
    }
}