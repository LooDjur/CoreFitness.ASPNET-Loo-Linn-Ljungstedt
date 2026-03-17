using Domain.Bookings.Enum;
using Domain.Bookings.ValueObjects;
using Domain.Common;
using Domain.Common.ValueObjects.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Bookings.Entity;

public sealed class BookingEntity : BaseEntity, IAggregateRoot
{
    public SessionId SessionId { get; private set; } = null!;
    public MemberId MemberId { get; private set; } = null!;
    public BookingStatus Status { get; private set; }

    private BookingEntity() { }

    private BookingEntity(SessionId sessionId, MemberId memberId)
    {
        SessionId = sessionId;
        MemberId = memberId;
        Status = BookingStatus.Confirmed;
    }

    public static Result<BookingEntity> Create(SessionId sessionId, MemberId memberId)
    {
        if (sessionId is null || sessionId.Value == Guid.Empty)
            return Result.Failure<BookingEntity>(DomainErrors.Session.NotFound);

        if (memberId is null || memberId.Value == Guid.Empty)
            return Result.Failure<BookingEntity>(DomainErrors.Membership.NotFound);

        var booking = new BookingEntity(sessionId, memberId);
        return Result.Success(booking);
    }

    public Result Cancel()
    {
        if (Status == BookingStatus.Cancelled)
            return Result.Failure(DomainErrors.Session.ActionNotAllowed);

        Status = BookingStatus.Cancelled;
        Modified = DateTime.UtcNow;

        return Result.Success();
    }
}