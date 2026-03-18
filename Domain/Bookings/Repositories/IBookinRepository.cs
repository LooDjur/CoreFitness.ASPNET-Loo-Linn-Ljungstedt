using Domain.Bookings.Entity;
using Domain.Common.Abstractions;
using Domain.Sessions.Enums;

namespace Domain.Bookings.Repositories;

public interface IBookingRepository : IRepositoryBase<BookingEntity>
{
    Task<bool> IsAlreadyBookedAsync(Guid sessionId, string memberId, CancellationToken ct = default);

    Task<IEnumerable<BookingEntity>> GetByMemberIdAsync(string memberId, CancellationToken ct = default);
}