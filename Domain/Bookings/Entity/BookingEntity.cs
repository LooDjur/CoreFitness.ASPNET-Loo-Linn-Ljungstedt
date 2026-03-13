using Domain.Bookings.Enum;
using Domain.Bookings.ValueObjects;
using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Bookings.Entity;

public sealed class BookingEntity : BaseEntity, IAggregateRoot
{
    public BookingSessionId SessionId { get; private set; } = null!;
    public BookingMemberId MemberId { get; private set; } = null!;
    public DateTime BookedAt { get; private set; }
    public BookingStatus Status { get; private set; }

    private BookingEntity() { }

    private BookingEntity(BookingSessionId sessionId, BookingMemberId memberId)
    {
        SessionId = sessionId;
        MemberId = memberId;
        BookedAt = DateTime.UtcNow;
        Status = BookingStatus.Confirmed;
    }

    public static Result<BookingEntity> Create(BookingSessionId sessionId, BookingMemberId memberId)
    {
        if (sessionId is null || memberId is null)
        {
            return Result.Failure<BookingEntity>("Booking requires both a valid session and member.");
        }

        var booking = new BookingEntity(sessionId, memberId);
        return booking;
    }

    public Result Cancel()
    {
        if (Status == BookingStatus.Cancelled)
        {
            return Result.Failure("Booking is already cancelled.");
        }

        Status = BookingStatus.Cancelled;
        Modified = DateTime.UtcNow;

        return Result.Success();
    }
}