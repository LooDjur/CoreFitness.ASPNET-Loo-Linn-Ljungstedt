using Domain.Bookings.Entity;
using Domain.Bookings.Repositories;
using Domain.Bookings.ValueObjects;
using Domain.Common.ValueObjects.Shared;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public sealed class BookingRepository(ApplicationDbContext context) : IBookingRepository
{
    public async Task<BookingEntity?> GetByIdAsync(BookingId id, CancellationToken ct = default)
        => await context.Bookings.FirstOrDefaultAsync(b => b.Id == id, ct);

    public async Task<IEnumerable<BookingEntity>> GetAllAsync(CancellationToken ct = default)
        => await context.Bookings.ToListAsync(ct);

    public async Task AddAsync(BookingEntity entity, CancellationToken ct = default)
        => await context.Bookings.AddAsync(entity, ct);

    public void Update(BookingEntity entity)
        => context.Bookings.Update(entity);

    public void Delete(BookingEntity entity)
    {
        entity.Delete(DateTime.UtcNow);
        context.Bookings.Update(entity);
    }

    public async Task DeleteAllByUserIdAsync(UserId userId, CancellationToken ct)
    {
        var userBookings = await context.Bookings
            .Where(b => context.Memberships
                .Any(m => m.UserId == userId && m.Id == b.MemberId))
            .ToListAsync(ct);

        context.Bookings.RemoveRange(userBookings);
    }

    public async Task<int> CountForSessionAsync(Guid sessionId, CancellationToken ct = default)
    {
        var sId = SessionId.Create(sessionId).Value;
        return await context.Bookings
            .CountAsync(b => b.SessionId == sId, ct);
    }

    public async Task<IEnumerable<BookingEntity>> GetByMemberIdAsync(MemberId memberId, CancellationToken ct = default)
    {
        return await context.Bookings
            .Where(b => b.MemberId == memberId && !b.IsDeleted)
            .ToListAsync(ct);
    }

    public async Task<bool> IsAlreadyBookedAsync(SessionId sessionId, MemberId memberId, CancellationToken ct = default)
    {
        return await context.Bookings
            .AnyAsync(b => b.SessionId == sessionId && b.MemberId == memberId, ct);
    }

    public async Task<BookingEntity?> GetSpecificBookingAsync(SessionId sessionId, MemberId memberId, CancellationToken ct)
    {
        return await context.Bookings
            .FirstOrDefaultAsync(b => b.SessionId == sessionId && b.MemberId == memberId, ct);
    }
}
