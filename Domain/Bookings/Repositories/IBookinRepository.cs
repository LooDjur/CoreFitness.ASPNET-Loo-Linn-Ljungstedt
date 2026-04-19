using Domain.Bookings.Entity;
using Domain.Bookings.ValueObjects;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;

namespace Domain.Bookings.Repositories;

public interface IBookingRepository : IRepositoryBase<BookingEntity, BookingId>
{
    Task DeleteAllByUserIdAsync(UserId userId, CancellationToken ct);
    Task<int> CountForSessionAsync(Guid sessionId, CancellationToken ct = default);
    Task<IEnumerable<BookingEntity>> GetByMemberIdAsync(MemberId memberId, CancellationToken ct = default);
    Task<bool> IsAlreadyBookedAsync(SessionId sessionId, MemberId memberId, CancellationToken ct = default);
    Task<BookingEntity?> GetSpecificBookingAsync(SessionId sessionId, MemberId memberId, CancellationToken ct);
}