using Application.Booking;
using Application.Sessions.Output;
using Domain.Bookings.Entity;
using Domain.Common;
using Domain.Common.ValueObjects.Shared;
using Domain.Sessions;
using Domain.Users.Entities;
using Domain.Users.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Xunit;

namespace Tests.Booking.Functionality;

public class GetMyBookingsHandlerTests : BaseIntegrationTest
{
    private readonly GetMyBookingsHandler _handler;

    public GetMyBookingsHandlerTests()
    {
        _handler = new GetMyBookingsHandler(UnitOfWork);
    }

    [Fact]
    public async Task Handle_Should_ReturnOnlyFutureBookedSessions()
    {
        // --- 1. Arrange ---
        var utcNow = DateTime.UtcNow;
        var user = CreateUserWithMembership(utcNow);

        var futureSession = CreateSession(utcNow.AddDays(1));
        var pastSession = CreateSession(utcNow.AddDays(2));

        Context.Set<UserEntity>().Add(user);
        Context.Set<SessionEntity>().AddRange(futureSession, pastSession);
        await Context.SaveChangesAsync();

        var pastStartTime = utcNow.AddDays(-1);
        var pastEndTime = pastStartTime.AddHours(1);

        await Context.Set<SessionEntity>()
            .Where(s => s.Id == pastSession.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.Schedule.StartTime, pastStartTime)
                .SetProperty(p => p.Schedule.EndTime, pastEndTime));

        var booking1 = BookingEntity.Create(futureSession.Id, user.Membership!.Id, utcNow).Value;
        var booking2 = BookingEntity.Create(pastSession.Id, user.Membership!.Id, utcNow).Value;

        Context.Set<BookingEntity>().AddRange(booking1, booking2);
        await Context.SaveChangesAsync();

        Context.ChangeTracker.Clear();

        var query = new GetMyBookingsQuery(user.Id.Value);

        // --- 2. Act ---
        var result = await _handler.Handle(query, CancellationToken.None);

        // --- 3. Assert ---
        result.IsSuccess.Should().BeTrue();

        result.Value.Should().HaveCount(1);
        result.Value.First().Id.Should().Be(futureSession.Id.Value);
        result.Value.First().IsBookedByCurrentUser.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyList_WhenUserHasNoBookings()
    {
        // --- 1. Arrange ---
        var utcNow = DateTime.UtcNow;
        var user = CreateUserWithMembership(utcNow);
        var session = CreateSession(utcNow.AddDays(1));

        Context.Set<UserEntity>().Add(user);
        Context.Set<SessionEntity>().Add(session);
        await Context.SaveChangesAsync();

        Context.ChangeTracker.Clear();

        var query = new GetMyBookingsQuery(user.Id.Value);

        // --- 2. Act ---
        var result = await _handler.Handle(query, CancellationToken.None);

        // --- 3. Assert ---
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_NotReturnOtherUsersBookings()
    {
        // --- 1. Arrange ---
        var utcNow = DateTime.UtcNow;
        var userA = CreateUserWithMembership(utcNow);
        var userB = CreateUserWithMembership(utcNow);
        var session = CreateSession(utcNow.AddDays(1));

        Context.Set<UserEntity>().AddRange(userA, userB);
        Context.Set<SessionEntity>().Add(session);
        await Context.SaveChangesAsync();

        var bookingForB = BookingEntity.Create(session.Id, userB.Membership!.Id, utcNow).Value;
        Context.Set<BookingEntity>().Add(bookingForB);
        await Context.SaveChangesAsync();

        Context.ChangeTracker.Clear();

        var query = new GetMyBookingsQuery(userA.Id.Value);

        // --- 2. Act ---
        var result = await _handler.Handle(query, CancellationToken.None);

        // --- 3. Assert ---
        result.Value.Should().BeEmpty();
    }

    private static UserEntity CreateUserWithMembership(DateTime utcNow)
    {
        var userId = UserId.Create(Guid.NewGuid()).Value;
        var email = Email.Create($"{Guid.NewGuid()}@test.com").Value;
        var user = UserEntity.Register(userId, email, utcNow);
        user.StartMembership(MembershipType.Premium, utcNow);
        return user;
    }

    private static SessionEntity CreateSession(DateTime startTime)
    {
        var timeSlot = TimeSlot.Create(startTime, startTime.AddHours(1)).Value;

        return SessionEntity.Create(
            Title.Create("Yoga").Value,
            Description.Create("Beskrivning").Value,
            Instructor.Create("Anna").Value,
            SessionCategory.Yoga,
            timeSlot,
            Capacity.Create(10).Value,
            DateTime.UtcNow).Value;
    }
}