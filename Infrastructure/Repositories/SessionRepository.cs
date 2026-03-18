using Domain.Sessions.Entities;
using Domain.Sessions.Enums;
using Domain.Sessions.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class SessionRepository(ApplicationDbContext context) : ISessionRepository
{
    public async Task<SessionEntity?> GetByGuidIdAsync(Guid id, CancellationToken ct)
        => await context.Sessions.FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<SessionEntity?> GetByIdAsync(string id, CancellationToken ct)
        => Guid.TryParse(id, out var guid) ? await GetByGuidIdAsync(guid, ct) : null;

    public async Task<IEnumerable<SessionEntity>> GetAllAsync(CancellationToken ct)
        => await context.Sessions.ToListAsync(ct);

    public async Task AddAsync(SessionEntity entity, CancellationToken ct)
        => await context.Sessions.AddAsync(entity, ct);

    public void Update(SessionEntity entity) => context.Sessions.Update(entity);

    public void Delete(SessionEntity entity) => context.Sessions.Remove(entity);

    public async Task<IEnumerable<SessionEntity>> GetUpcomingSessionsAsync(CancellationToken ct)
        => await context.Sessions
            .Where(s => s.Schedule.StartTime > DateTime.UtcNow && !s.IsDeleted)
            .ToListAsync(ct);

    public async Task<IEnumerable<SessionEntity>> GetByCategoryAsync(SessionCategory category, CancellationToken ct)
        => await context.Sessions.Where(s => s.Category == category).ToListAsync(ct);
}