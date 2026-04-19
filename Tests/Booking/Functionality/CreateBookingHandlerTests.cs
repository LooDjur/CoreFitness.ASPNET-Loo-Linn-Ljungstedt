using Application.Booking;
using Domain.Bookings.Entity;
using Domain.Bookings.ValueObjects;
using Domain.Common;
using Domain.Common.ValueObjects.Shared;
using Domain.Sessions;
using Domain.Users.Entities;
using Domain.Users.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;

namespace Tests.Booking.Functionality;

public class CreateBookingHandlerTests : BaseIntegrationTest
{
    private readonly CreateBookingCommandHandler _handler;

    public CreateBookingHandlerTests()
    {
        _handler = new CreateBookingCommandHandler(UnitOfWork);
    }

    [Fact]
    public async Task Handle_Should_CreateBooking_WhenDataIsValid()
    {
        // --- 1. Arrange ---
        var utcNow = DateTime.UtcNow;
        var user = CreateUserWithMembership(utcNow);
        var session = CreateFutureSession(utcNow);

        Context.Set<UserEntity>().Add(user);
        Context.Set<SessionEntity>().Add(session);
        await Context.SaveChangesAsync();

        Context.ChangeTracker.Clear();

        var command = new CreateBookingCommand(session.Id.Value, user.Id.Value, utcNow);

        // --- 2. Act ---
        var result = await _handler.Handle(command, CancellationToken.None);

        // --- 3. Assert ---
        result.IsSuccess.Should().BeTrue();
        Context.ChangeTracker.Clear();

        var createdBookingId = BookingId.Create(result.Value).Value;
        var bookingInDb = await Context.Set<BookingEntity>()
            .FirstOrDefaultAsync(b => b.Id == createdBookingId);

        bookingInDb.Should().NotBeNull();
        bookingInDb!.MemberId.Value.Should().Be(user.Membership!.Id.Value);
        bookingInDb.SessionId.Value.Should().Be(session.Id.Value);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenUserAlreadyBooked()
    {
        // --- 1. Arrange ---
        var utcNow = DateTime.UtcNow;
        var user = CreateUserWithMembership(utcNow);
        var session = CreateFutureSession(utcNow);

        Context.Set<UserEntity>().Add(user);
        Context.Set<SessionEntity>().Add(session);
        await Context.SaveChangesAsync();

        var existingBooking = BookingEntity.Create(session.Id, user.Membership!.Id, utcNow).Value;
        Context.Set<BookingEntity>().Add(existingBooking);
        await Context.SaveChangesAsync();

        Context.ChangeTracker.Clear();

        var command = new CreateBookingCommand(session.Id.Value, user.Id.Value, utcNow);

        // --- 2. Act ---
        var result = await _handler.Handle(command, CancellationToken.None);

        // --- 3. Assert ---
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Session.ActionNotAllowed);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenUserHasNoMembership()
    {
        // --- 1. Arrange ---
        var utcNow = DateTime.UtcNow;
        var userId = UserId.Create(Guid.NewGuid()).Value;
        var userWithoutMembership = UserEntity.Register(
            userId,
            Email.Create("unlucky@test.se").Value,
            utcNow);

        var session = CreateFutureSession(utcNow);

        Context.Set<UserEntity>().Add(userWithoutMembership);
        Context.Set<SessionEntity>().Add(session);
        await Context.SaveChangesAsync();

        Context.ChangeTracker.Clear();

        var command = new CreateBookingCommand(session.Id.Value, userWithoutMembership.Id.Value, utcNow);

        // --- 2. Act ---
        var result = await _handler.Handle(command, CancellationToken.None);

        // --- 3. Assert ---
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.Ineligible);
    }

    // --- Helpers ---

    private static UserEntity CreateUserWithMembership(DateTime utcNow)
    {
        var userId = UserId.Create(Guid.NewGuid()).Value;
        var email = Email.Create($"{Guid.NewGuid()}@test.com").Value;
        var user = UserEntity.Register(userId, email, utcNow);

        user.StartMembership(MembershipType.Premium, utcNow);

        return user;
    }

    private static SessionEntity CreateFutureSession(DateTime utcNow)
    {
        return SessionEntity.Create(
            Title.Create("Yoga Flow").Value,
            Description.Create("Ett lugnt pass").Value,
            Instructor.Create("Anna").Value,
            SessionCategory.Yoga,
            TimeSlot.Create(utcNow.AddDays(1), utcNow.AddDays(1).AddHours(1)).Value,
            Capacity.Create(10).Value,
            utcNow).Value;
    }
}