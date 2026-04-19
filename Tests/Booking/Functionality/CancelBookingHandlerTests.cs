using Application.Booking;
using Domain.Bookings.Entity;
using Domain.Common;
using Domain.Common.ValueObjects.Shared;
using Domain.Sessions;
using Domain.Users.Entities;
using Domain.Users.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;

namespace Tests.Booking.Functionality;

public class CancelBookingHandlerTests : BaseIntegrationTest
{
    private readonly CancelBookingCommandHandler _handler;

    public CancelBookingHandlerTests()
    {
        _handler = new CancelBookingCommandHandler(
            UnitOfWork,
            UnitOfWork.Bookings,
            UnitOfWork.Users);
    }

    [Fact]
    public async Task Handle_Should_CancelBooking_WhenBookingExists()
    {
        // --- 1. Arrange ---
        var utcNow = DateTime.UtcNow;
        var user = CreateUserWithMembership(utcNow);
        var session = CreateFutureSession(utcNow);

        var booking = BookingEntity.Create(session.Id, user.Membership!.Id, utcNow).Value;

        Context.Set<UserEntity>().Add(user);
        Context.Set<SessionEntity>().Add(session);
        Context.Set<BookingEntity>().Add(booking);
        await Context.SaveChangesAsync();

        // Rensa trackern så handlern måste hämta user/membership/booking från DB
        Context.ChangeTracker.Clear();

        var command = new CancelBookingCommand(session.Id.Value, user.Id.Value);

        // --- 2. Act ---
        var result = await _handler.Handle(command, CancellationToken.None);

        // --- 3. Assert ---
        result.IsSuccess.Should().BeTrue();

        // Rensa igen för att verifiera slutresultatet i DB utan minnes-cache
        Context.ChangeTracker.Clear();

        var bookingInDb = await Context.Set<BookingEntity>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(b => b.Id == booking.Id);

        // Om din repository tar bort raden helt (Hard Delete) ska bookingInDb vara null
        // Om du kör Soft Delete ska IsDeleted vara true
        if (bookingInDb is not null)
        {
            bookingInDb.IsDeleted.Should().BeTrue();
        }
        else
        {
            bookingInDb.Should().BeNull();
        }
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenBookingDoesNotExist()
    {
        // --- 1. Arrange ---
        var utcNow = DateTime.UtcNow;
        var user = CreateUserWithMembership(utcNow);
        var session = CreateFutureSession(utcNow);

        Context.Set<UserEntity>().Add(user);
        Context.Set<SessionEntity>().Add(session);
        await Context.SaveChangesAsync();

        Context.ChangeTracker.Clear();

        var command = new CancelBookingCommand(session.Id.Value, user.Id.Value);

        // --- 2. Act ---
        var result = await _handler.Handle(command, CancellationToken.None);

        // --- 3. Assert ---
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Booking.NotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenUserNotFound()
    {
        // --- 1. Arrange ---
        // Ingen data läggs till = Tom databas
        var command = new CancelBookingCommand(Guid.NewGuid(), Guid.NewGuid());

        // --- 2. Act ---
        var result = await _handler.Handle(command, CancellationToken.None);

        // --- 3. Assert ---
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.NotFound);
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
            Title.Create("Pass").Value,
            Description.Create("Beskrivning").Value,
            Instructor.Create("PT-Kalle").Value,
            SessionCategory.Yoga,
            TimeSlot.Create(utcNow.AddDays(1), utcNow.AddDays(1).AddHours(1)).Value,
            Capacity.Create(10).Value,
            utcNow).Value;
    }
}
