using Application.Abstractions.Authentication;
using Application.Users.Commands.Delete.User;
using Domain.Bookings.Entity;
using Domain.Common;
using Domain.Common.ValueObjects.Shared;
using Domain.Users.Entities;
using Domain.Users.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Tests.Common;

namespace Tests.Users.User.Functionality;

public class DeleteUserHandlerTests : BaseIntegrationTest
{
    private readonly IAuthService _authServiceMock;
    private readonly DeleteUserCommandHandler _handler;

    public DeleteUserHandlerTests()
    {
        _authServiceMock = Substitute.For<IAuthService>();
        _handler = new DeleteUserCommandHandler(UnitOfWork, _authServiceMock);
    }

    [Fact]
    public async Task Handle_Should_DeleteUserAndIdentity_WhenUserExists()
    {
        // --- 1. Arrange ---
        var userId = UserId.Create(Guid.NewGuid()).Value;
        var email = Email.Create("delete-me@test.se").Value;
        var utcNow = DateTime.UtcNow;

        var user = UserEntity.Register(userId, email, utcNow);

        var membership = MembershipEntity.CreateInternal(userId, MembershipType.Standard, utcNow).Value;

        user.GetType().GetProperty("Membership")?.SetValue(user, membership);

        var session = Domain.Sessions.SessionEntity.Create(
            Domain.Sessions.Title.Create("Yoga").Value,
            Domain.Sessions.Description.Create("Beskrivning").Value,
            Domain.Sessions.Instructor.Create("Anna").Value,
            Domain.Sessions.SessionCategory.Yoga,
            Domain.Sessions.TimeSlot.Create(utcNow.AddDays(1), utcNow.AddDays(2)).Value,
            Domain.Sessions.Capacity.Create(15).Value,
            utcNow).Value;

        var booking = BookingEntity.Create(session.Id, membership.Id, utcNow).Value;

        Context.Set<UserEntity>().Add(user);
        Context.Set<MembershipEntity>().Add(membership);
        Context.Set<Domain.Sessions.SessionEntity>().Add(session);
        Context.Set<BookingEntity>().Add(booking);
        await Context.SaveChangesAsync();

        _authServiceMock.DeleteIdentityUserAsync(userId.Value, Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var command = new DeleteUserCommand(userId.Value);

        // --- 2. Act ---
        var result = await _handler.Handle(command, CancellationToken.None);

        // --- 3. Assert ---
        result.IsSuccess.Should().BeTrue();

        await _authServiceMock.Received(1).DeleteIdentityUserAsync(userId.Value, Arg.Any<CancellationToken>());

        Context.ChangeTracker.Clear();

        var deletedUser = await Context.Set<UserEntity>().FirstOrDefaultAsync(u => u.Id == userId);
        deletedUser.Should().BeNull();

        var remainingBookings = await Context.Set<BookingEntity>()
            .Where(b => b.MemberId == membership.Id)
            .ToListAsync();
        remainingBookings.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenUserNotFound()
    {
        // --- Arrange ---
        var command = new DeleteUserCommand(Guid.NewGuid());

        // --- Act ---
        var result = await _handler.Handle(command, CancellationToken.None);

        // --- Assert ---
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.NotFound);
    }
}