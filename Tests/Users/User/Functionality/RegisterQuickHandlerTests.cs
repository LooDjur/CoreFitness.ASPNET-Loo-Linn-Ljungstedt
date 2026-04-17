using Application.Users.Commands.Create.User;
using Domain.Common;
using Domain.Users.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Tests.Common;

namespace Tests.Users.User.Functionality;

public class RegisterQuickHandlerTests : BaseIntegrationTest
{
    [Fact]
    public async Task Handle_Should_CreateUserInDatabase_When_IdentityIsSuccessful()
    {
        // Arrange
        var command = new RegisterQuickCommand("new@user.se", "Pass123!", "Member", DateTime.UtcNow);
        var identityId = Guid.NewGuid();

        // Vi mockar Identity-tjänsten att lyckas
        AuthServiceMock.RegisterIdentityAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), default)
            .Returns(Result.Success(identityId));

        var handler = new RegisterQuickHandler(UnitOfWork, AuthServiceMock);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var userInDb = await Context.Set<UserEntity>().FirstOrDefaultAsync(u => u.Email.Value == "new@user.se");
        userInDb.Should().NotBeNull();
        userInDb!.Id.Value.Should().Be(identityId);
    }
}