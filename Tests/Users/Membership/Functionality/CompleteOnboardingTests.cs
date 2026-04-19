using Application.Users.Commands.Create.Membership;
using Domain.Common.ValueObjects.Shared;
using Domain.Users.Entities;
using Domain.Users.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;

namespace Tests.Users.Membership.Functionality;

public class CompleteOnboardingTests : BaseIntegrationTest
{
    [Fact]
    public async Task Handle_Should_UpdateProfile_And_StartMembership()
    {
        // Arrange
        var userId = UserId.Create(Guid.NewGuid()).Value;
        var user = UserEntity.Register(userId, Email.Create("test@test.se").Value, DateTime.UtcNow);

        Context.Set<UserEntity>().Add(user);
        await Context.SaveChangesAsync();

        var command = new CompleteOnboardingCommand(userId.Value, "Kalle", "Anka", "+467000000", "Standard", DateTime.UtcNow);
        var handler = new CompleteOnboardingHandler(UnitOfWork);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        Context.ChangeTracker.Clear();

        var updatedUser = await Context.Set<UserEntity>()
            .Include(u => u.Membership)
            .FirstOrDefaultAsync(u => u.Id == userId);

        updatedUser!.FirstName!.Value.Should().Be("Kalle");
        updatedUser.Membership.Should().NotBeNull();
        updatedUser.Membership!.Status.Should().Be(MembershipStatus.Active);
    }
}