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

    public BookingEntity(BookingSessionId sessionId, BookingMemberId memberId)
    {
        SessionId = sessionId;
        MemberId = memberId;
        BookedAt = DateTime.UtcNow;
        Status = BookingStatus.Confirmed;
    }

    public void Cancel()
    {
        Status = BookingStatus.Cancelled;
        Modified = DateTime.UtcNow;
    }
}