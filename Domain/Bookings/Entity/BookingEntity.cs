using Domain.Bookings.Enum;
using Domain.Bookings.ValueObjects;
using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;

namespace Domain.Bookings.Entity;

public sealed class BookingEntity : BaseEntity<BookingId>, IAggregateRoot
{
    public SessionId SessionId { get; private set; } = null!;
    public UserId UserId { get; private set; } = null!;
    public BookingStatus Status { get; private set; }

    private BookingEntity() { }

    private BookingEntity(BookingId id, SessionId sessionId, UserId userId)
    {
        Id = id;
        SessionId = sessionId;
        UserId = userId;
        Status = BookingStatus.Confirmed;
    }

    public static Result<BookingEntity> Create(SessionId sessionId, UserId userId)
    {
        if (sessionId is null || sessionId.Value == Guid.Empty)
            return Result.Failure<BookingEntity>(DomainErrors.Session.NotFound);

        if (userId is null || userId.Value == Guid.Empty)
            return Result.Failure<BookingEntity>(DomainErrors.User.NotFound);

        var bookingId = BookingId.New();
        var booking = new BookingEntity(bookingId, sessionId, userId);

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