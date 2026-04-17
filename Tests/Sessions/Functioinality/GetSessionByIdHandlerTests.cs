using Application.Sessions.Queries;
using Domain.Bookings.Entity;
using Domain.Common;
using Domain.Common.ValueObjects.Shared;
using Domain.Sessions;
using Domain.Users.Entities;
using Domain.Users.Enums;
using FluentAssertions;
using Tests.Common;

namespace Tests.Sessions.Functioinality;

public class GetSessionByIdHandlerTests : BaseIntegrationTest
{
    private readonly GetSessionByIdHandler _handler;

    public GetSessionByIdHandlerTests()
    {
        _handler = new GetSessionByIdHandler(UnitOfWork);
    }

    [Fact]
    public async Task Handle_Should_ReturnSessionWithIsBookedFalse_WhenUserIdIsNull()
    {
        // --- 1. Arrange ---
        var session = CreateAndPersistSession();

        Context.ChangeTracker.Clear();
        var query = new GetSessionByIdQuery(session.Id.Value, null);

        // --- 2. Act ---
        var result = await _handler.Handle(query, CancellationToken.None);

        // --- 3. Assert ---
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(session.Id.Value);
        result.Value.IsBookedByCurrentUser.Should().BeFalse();
        result.Value.Title.Should().Be(session.Title.Value);
    }

    [Fact]
    public async Task Handle_Should_ReturnIsBookedTrue_WhenUserHasActiveBooking()
    {
        // --- 1. Arrange ---
        var session = CreateAndPersistSession();

        var userId = UserId.Create(Guid.NewGuid()).Value;
        var user = UserEntity.Register(userId, Email.Create("test@test.se").Value, DateTime.UtcNow);

        user.StartMembership(MembershipType.Standard, DateTime.UtcNow);

        Context.Set<UserEntity>().Add(user);
        await Context.SaveChangesAsync();

        var booking = BookingEntity.Create(
            session.Id,
            user.Membership!.Id,
            DateTime.UtcNow).Value;

        Context.Set<BookingEntity>().Add(booking);
        await Context.SaveChangesAsync();

        Context.ChangeTracker.Clear();
        var query = new GetSessionByIdQuery(session.Id.Value, user.Id.Value);

        // --- 2. Act ---
        var result = await _handler.Handle(query, CancellationToken.None);

        // --- 3. Assert ---
        result.IsSuccess.Should().BeTrue();
        result.Value.IsBookedByCurrentUser.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_WhenSessionIsDeleted()
    {
        // --- 1. Arrange ---
        var session = CreateAndPersistSession();
        session.Delete(DateTime.UtcNow);
        await Context.SaveChangesAsync();

        Context.ChangeTracker.Clear();
        var query = new GetSessionByIdQuery(session.Id.Value, null);

        // --- 2. Act ---
        var result = await _handler.Handle(query, CancellationToken.None);

        // --- 3. Assert ---
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Session.NotFound);
    }

    private SessionEntity CreateAndPersistSession()
    {
        var start = DateTime.UtcNow.AddDays(1);
        var session = SessionEntity.Create(
            Title.Create("Yoga Flow").Value,
            Description.Create("Description").Value,
            Instructor.Create("Anna").Value,
            SessionCategory.Yoga,
            TimeSlot.Create(start, start.AddHours(1)).Value,
            Capacity.Create(15).Value,
            DateTime.UtcNow).Value;

        Context.Set<SessionEntity>().Add(session);
        Context.SaveChanges();
        return session;
    }
}