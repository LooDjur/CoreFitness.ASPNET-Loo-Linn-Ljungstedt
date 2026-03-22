using Domain.Bookings.Entity;
using Domain.Bookings.ValueObjects;
using Domain.Common.Abstractions;
using Domain.Sessions.Enums;

namespace Domain.Bookings.Repositories;

public interface IBookingRepository : IRepositoryBase<BookingEntity, BookingId>
{
    Task<bool> IsAlreadyBookedAsync(Guid sessionId, string memberId, CancellationToken ct = default);

    Task<IEnumerable<BookingEntity>> GetByMemberIdAsync(string memberId, CancellationToken ct = default);
}