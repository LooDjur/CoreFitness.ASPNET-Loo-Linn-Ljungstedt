using Domain.Bookings.Entity;
using Domain.Bookings.Enum;
using Domain.Common;
using Domain.Common.ValueObjects.Shared;
using FluentAssertions;
using Xunit;

namespace Tests.Booking;

public class BookingEntityTests
{
    private readonly SessionId _sessionId = SessionId.Create(Guid.NewGuid()).Value;
    private readonly MemberId _memberId = MemberId.Create(Guid.NewGuid()).Value;
    private readonly DateTime _utcNow = DateTime.UtcNow;

    [Fact]
    public void Create_Should_ReturnSuccess_WhenDataIsValid()
    {
        // --- Act ---
        var result = BookingEntity.Create(_sessionId, _memberId, _utcNow);

        // --- Assert ---
        result.IsSuccess.Should().BeTrue();
        result.Value.SessionId.Should().Be(_sessionId);
        result.Value.MemberId.Should().Be(_memberId);
        result.Value.Status.Should().Be(BookingStatus.Confirmed);
        result.Value.Id.Should().NotBeNull();
    }

    [Fact]
    public void Create_Should_SetInitialStatusToConfirmed()
    {
        // --- Act ---
        var result = BookingEntity.Create(_sessionId, _memberId, _utcNow);

        // --- Assert ---
        result.Value.Status.Should().Be(BookingStatus.Confirmed);
    }

    [Fact]
    public void Delete_Should_SetStatusToCancelledAndMarkAsDeleted()
    {
        // --- Arrange ---
        var booking = BookingEntity.Create(_sessionId, _memberId, _utcNow).Value;
        var deleteTime = _utcNow.AddHours(1);

        // --- Act ---
        var result = booking.Delete(deleteTime);

        // --- Assert ---
        result.IsSuccess.Should().BeTrue();
        booking.Status.Should().Be(BookingStatus.Cancelled);
        booking.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void Delete_Should_ReturnFailure_WhenAlreadyDeleted()
    {
        // --- Arrange ---
        var booking = BookingEntity.Create(_sessionId, _memberId, _utcNow).Value;
        booking.Delete(_utcNow);

        // --- Act ---
        var result = booking.Delete(_utcNow);

        // --- Assert ---
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Session.ActionNotAllowed);
    }

    [Fact]
    public void Create_Should_ReturnFailure_WhenSessionIdIsNull()
    {
        // --- Act ---
        var result = BookingEntity.Create(null!, _memberId, _utcNow);

        // --- Assert ---
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Session.NotFound);
    }

    [Fact]
    public void Create_Should_ReturnFailure_WhenMemberIdIsNull()
    {
        // --- Act ---
        var result = BookingEntity.Create(_sessionId, null!, _utcNow);

        // --- Assert ---
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.NotFound);
    }
}