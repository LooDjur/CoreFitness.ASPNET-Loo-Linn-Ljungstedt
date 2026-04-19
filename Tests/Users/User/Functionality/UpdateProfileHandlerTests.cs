using Application.Abstractions.Authentication;
using Application.Users.Commands.Update;
using Domain.Common;
using Domain.Common.ValueObjects.Shared;
using Domain.Users.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Tests.Common;

namespace Tests.Users.User.Functionality;

public class UpdateProfileHandlerTests : BaseIntegrationTest
{
    private readonly IAuthService _authServiceMock;
    private readonly UpdateProfileHandler _handler;

    public UpdateProfileHandlerTests()
    {
        _authServiceMock = Substitute.For<IAuthService>();
        _handler = new UpdateProfileHandler(UnitOfWork, _authServiceMock);
    }

    [Fact]
    public async Task Handle_Should_UpdateUserProfile_WhenDataIsValid()
    {
        // --- 1. Arrange ---
        var userId = UserId.Create(Guid.NewGuid()).Value;
        var originalEmail = Email.Create("old@fitness.se").Value;
        var utcNow = DateTime.UtcNow;

        var user = UserEntity.Register(userId, originalEmail, utcNow);
        Context.Set<UserEntity>().Add(user);
        await Context.SaveChangesAsync();

        var newEmail = "new@fitness.se";
        var newFirstName = "Anders";
        var newLastName = "Andersson";
        var newPhone = "0701234567";
        var newImageUrl = "https://cdn.se/image.jpg";

        var command = new UpdateProfileCommand(
            userId.Value,
            newFirstName,
            newLastName,
            newEmail,
            newPhone,
            newImageUrl,
            utcNow);

        // Mocka lyckad uppdatering i Identity
        _authServiceMock.UpdateIdentityUserAsync(
            Arg.Any<Guid>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // --- 2. Act ---
        var result = await _handler.Handle(command, CancellationToken.None);

        // --- 3. Assert ---
        result.IsSuccess.Should().BeTrue();

        // Verifiera att AuthService anropades med rätt värden
        await _authServiceMock.Received(1).UpdateIdentityUserAsync(
            userId.Value,
            newEmail,
            newFirstName,
            newLastName,
            newPhone,
            Arg.Any<CancellationToken>());

        Context.ChangeTracker.Clear();

        var updatedUser = await Context.Set<UserEntity>().FirstOrDefaultAsync(u => u.Id == userId);

        updatedUser.Should().NotBeNull();
        updatedUser!.Email.Value.Should().Be(newEmail);
        updatedUser.FirstName!.Value.Should().Be(newFirstName);
        updatedUser.LastName!.Value.Should().Be(newLastName);
        updatedUser.Phone!.Value.Should().Be(newPhone);
        updatedUser.ProfileImageUrl.Should().Be(newImageUrl);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenEmailIsAlreadyTaken()
    {
        // --- 1. Arrange ---
        var utcNow = DateTime.UtcNow;

        // Skapa befintlig användare 1 (den vi ska uppdatera)
        var user1 = UserEntity.Register(UserId.Create(Guid.NewGuid()).Value, Email.Create("user1@test.se").Value, utcNow);

        // Skapa befintlig användare 2 (som redan har den eftertraktade mailadressen)
        var takenEmail = "taken@test.se";
        var user2 = UserEntity.Register(UserId.Create(Guid.NewGuid()).Value, Email.Create(takenEmail).Value, utcNow);

        Context.Set<UserEntity>().AddRange(user1, user2);
        await Context.SaveChangesAsync();

        var command = new UpdateProfileCommand(
            user1.Id.Value,
            "Anders",
            "Andersson",
            takenEmail, // Försöker ta användare 2:s mail
            "0701234567",
            null,
            utcNow);

        // --- 2. Act ---
        var result = await _handler.Handle(command, CancellationToken.None);

        // --- 3. Assert ---
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.EmailInvalid);
    }
}