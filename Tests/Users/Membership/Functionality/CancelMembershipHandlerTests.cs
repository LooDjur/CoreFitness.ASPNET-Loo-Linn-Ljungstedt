using Application.Users.Commands.Delete.Membership;
using Domain.Bookings.Entity;
using Domain.Common.ValueObjects.Shared;
using Domain.Sessions;
using Domain.Users.Entities;
using Domain.Users.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;

namespace Tests.Users.Membership.Functionality;

public class CancelMembershipTest : BaseIntegrationTest
{
    [Fact]
    public async Task Handle_Should_CancelMembership_And_SoftDeleteAllRelatedBookings()
    {
        // --- 1. Arrange ---
        var userId = UserId.Create(Guid.NewGuid()).Value;
        var email = Email.Create("test@fitness.se").Value;
        var utcNow = DateTime.UtcNow;

        var user = UserEntity.Register(userId, email, utcNow);
        var membership = MembershipEntity.CreateInternal(user.Id, MembershipType.Standard, utcNow).Value;

        // Nu kan vi skriva koden kort och koncist tack vare using-satserna
        var timeSlot = TimeSlot.Create(utcNow.AddDays(1), utcNow.AddDays(1).AddHours(1)).Value;
        var capacity = Capacity.Create(10).Value;

        // Här skapar vi de ValueObjects som SessionEntity.Create kräver
        var title = Title.Create("Yoga").Value;
        var description = Description.Create("Desc").Value;
        var instructor = Instructor.Create("Instructor").Value;

        var session = SessionEntity.Create(
            title,
            description,
            instructor,
            SessionCategory.Yoga,
            timeSlot,
            capacity,
            utcNow).Value;

        var booking = BookingEntity.Create(session.Id, membership.Id, utcNow).Value;

        Context.Set<UserEntity>().Add(user);
        Context.Set<MembershipEntity>().Add(membership);
        Context.Set<SessionEntity>().Add(session);
        Context.Set<BookingEntity>().Add(booking);
        await Context.SaveChangesAsync();

        var command = new CancelMembershipCommand(userId.Value, utcNow);
        var handler = new CancelMembershipHandler(UnitOfWork);

        // --- 2. Act ---
        var result = await handler.Handle(command, CancellationToken.None);

        // --- 3. Assert ---
        result.IsSuccess.Should().BeTrue();

        Context.ChangeTracker.Clear();

        var updatedUser = await Context.Set<UserEntity>()
            .Include(u => u.Membership)
            .FirstOrDefaultAsync(u => u.Id == userId);

        updatedUser!.Membership.Should().BeNull();

        // Vi kollar att det inte finns några AKTIVA bokningar kvar
        var activeBookings = await Context.Set<BookingEntity>()
            .Where(b => b.MemberId == membership.Id && !b.IsDeleted)
            .ToListAsync();

        activeBookings.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenUserHasNoMembership()
    {
        // --- Arrange ---
        var userId = UserId.Create(Guid.NewGuid()).Value;
        var email = Email.Create("no-membership@test.se").Value;
        var user = UserEntity.Register(userId, email, DateTime.UtcNow);

        Context.Set<UserEntity>().Add(user);
        await Context.SaveChangesAsync();

        var command = new CancelMembershipCommand(userId.Value, DateTime.UtcNow);
        var handler = new CancelMembershipHandler(UnitOfWork);

        // --- Act ---
        var result = await handler.Handle(command, CancellationToken.None);

        // --- Assert ---
        result.IsSuccess.Should().BeTrue();
    }
}