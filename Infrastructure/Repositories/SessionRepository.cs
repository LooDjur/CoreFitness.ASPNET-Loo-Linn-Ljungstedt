using Domain.Common.ValueObjects.Shared;
using Domain.Sessions;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class SessionRepository(ApplicationDbContext context) : ISessionRepository
{
    public async Task<SessionEntity?> GetByIdAsync(SessionId id, CancellationToken ct = default)
        => await context.Sessions.FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IEnumerable<SessionEntity>> GetAllAsync(CancellationToken ct = default)
        => await context.Sessions.ToListAsync(ct);

    public async Task AddAsync(SessionEntity entity, CancellationToken ct = default)
        => await context.Sessions.AddAsync(entity, ct);

    public void Update(SessionEntity entity) => context.Sessions.Update(entity);

    public void Delete(SessionEntity entity) => context.Sessions.Remove(entity);

    public async Task<IEnumerable<SessionEntity>> GetUpcomingSessionsAsync(CancellationToken ct = default)
        => await context.Sessions
            .Where(s => s.Schedule.StartTime > DateTime.UtcNow)
            .OrderBy(s => s.Schedule.StartTime)
            .ToListAsync(ct);

    public async Task<IEnumerable<SessionEntity>> GetByCategoryAsync(SessionCategory category, CancellationToken ct = default)
        => await context.Sessions.Where(s => s.Category == category).ToListAsync(ct);
}