using Domain.Common.ValueObjects.Shared;
using Domain.Sessions;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public sealed class SessionRepository(ApplicationDbContext context) : ISessionRepository
{
    public async Task<SessionEntity?> GetByIdAsync(SessionId id, CancellationToken ct = default)
        => await context.Sessions
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, ct);

    public async Task<IEnumerable<SessionEntity>> GetAllAsync(CancellationToken ct = default)
        => await context.Sessions
            .Where(s => !s.IsDeleted)
            .ToListAsync(ct);

    public async Task AddAsync(SessionEntity entity, CancellationToken ct = default)
        => await context.Sessions.AddAsync(entity, ct);

    public void Update(SessionEntity entity) => context.Sessions.Update(entity);

    public void Delete(SessionEntity entity) => context.Sessions.Remove(entity);

    public async Task<IEnumerable<SessionEntity>> GetUpcomingSessionsAsync(DateTime utcNow, CancellationToken ct = default)
    {
        return await context.Sessions
            .Where(s => !s.IsDeleted && s.Schedule.StartTime > utcNow)
            .OrderBy(s => s.Schedule.StartTime)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<SessionEntity>> GetByCategoryAsync(SessionCategory category, CancellationToken ct = default)
        => await context.Sessions
            .Where(s => !s.IsDeleted && s.Category == category)
            .ToListAsync(ct);
}