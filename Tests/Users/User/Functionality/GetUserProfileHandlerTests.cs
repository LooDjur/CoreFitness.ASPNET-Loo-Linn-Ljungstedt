using Application.Users.Queries;
using Domain.Common.ValueObjects.Shared;
using Domain.Users.Entities;
using Domain.Users.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;

namespace Tests.Users.User.Functionality;

public class GetUserProfileHandlerTests : BaseIntegrationTest
{
    private readonly GetUserProfileHandler _handler;

    public GetUserProfileHandlerTests()
    {
        _handler = new GetUserProfileHandler(UnitOfWork);
    }

    [Fact]
    public async Task Handle_Should_ReturnUserWithMembership_WhenUserExists()
    {
        // --- 1. Arrange ---
        var userId = UserId.Create(Guid.NewGuid()).Value;
        var email = Email.Create("member@test.se").Value;
        var utcNow = DateTime.UtcNow;

        var user = UserEntity.Register(userId, email, utcNow);

        var membership = MembershipEntity.CreateInternal(
            userId,
            MembershipType.Premium,
            utcNow.AddMonths(1)).Value;

        user.GetType().GetProperty("Membership")?.SetValue(user, membership);

        Context.Set<UserEntity>().Add(user);
        Context.Set<MembershipEntity>().Add(membership);
        await Context.SaveChangesAsync();

        var query = new GetUserProfileQuery(userId.Value, utcNow);

        // --- 2. Act ---
        var result = await _handler.Handle(query, CancellationToken.None);

        // --- 3. Assert ---
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Email.Should().Be(email.Value);

        result.Value.MembershipPlan.Should().Be("Premium");
        result.Value.IsActive.Should().BeTrue();
        result.Value.MemberId.Should().Be(membership.Id.Value);
        result.Value.UserId.Should().Be(userId.Value);
    }

    [Fact]
    public async Task Handle_Should_ReturnNoActivePlan_WhenUserHasNoMembership()
    {
        // --- 1. Arrange ---
        var userId = UserId.Create(Guid.NewGuid()).Value;
        var user = UserEntity.Register(userId, Email.Create("no-member@test.se").Value, DateTime.UtcNow);

        Context.Set<UserEntity>().Add(user);
        await Context.SaveChangesAsync();

        var query = new GetUserProfileQuery(userId.Value, DateTime.UtcNow);

        // --- 2. Act ---
        var result = await _handler.Handle(query, CancellationToken.None);

        // --- 3. Assert ---
        result.IsSuccess.Should().BeTrue();

        result.Value.MembershipPlan.Should().Be("No active plan");
        result.Value.MemberId.Should().BeNull();
        result.Value.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenUserNotFound()
    {
        // --- Arrange ---
        var query = new GetUserProfileQuery(Guid.NewGuid(), DateTime.UtcNow);

        // --- Act ---
        var result = await _handler.Handle(query, CancellationToken.None);

        // --- Assert ---
        result.IsFailure.Should().BeTrue();
    }
}