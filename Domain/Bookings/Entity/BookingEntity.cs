using Domain.Bookings.Enum;
using Domain.Bookings.ValueObjects;
using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;

namespace Domain.Bookings.Entity;

public sealed class BookingEntity : BaseEntity<BookingId>, IAggregateRoot
{
    public SessionId SessionId { get; private set; } = null!;
    public MemberId MemberId { get; private set; } = null!;
    public BookingStatus Status { get; private set; }

    private BookingEntity() { }

    private BookingEntity(BookingId id, SessionId sessionId, MemberId memberId, DateTime utcNow)
    {
        Initialize(id, utcNow);

        SessionId = sessionId;
        MemberId = memberId;
        Status = BookingStatus.Confirmed;
    }

    public static Result<BookingEntity> Create(SessionId sessionId, MemberId memberId, DateTime utcNow)
    {
        if (sessionId is null || sessionId.Value == Guid.Empty)
            return Result.Failure<BookingEntity>(DomainErrors.Session.NotFound);

        if (memberId is null || memberId.Value == Guid.Empty)
            return Result.Failure<BookingEntity>(DomainErrors.User.NotFound);

        var bookingId = BookingId.New();
        var booking = new BookingEntity(bookingId, sessionId, memberId, utcNow);
        return Result.Success(booking);
    }

    public Result Delete(DateTime utcNow)
    {
        if (IsDeleted)
            return Result.Failure(DomainErrors.Session.ActionNotAllowed);

        IsDeleted = true;
        Status = BookingStatus.Cancelled;
        UpdateModified(utcNow);

        return Result.Success();
    }
}