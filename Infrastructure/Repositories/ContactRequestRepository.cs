using Domain.Common;
using Domain.Common.ValueObjects.Shared;
using Domain.ContactReq.Entities;
using Domain.ContactReq.Repositories;
using Domain.ContactReq.ValueObjects;
using Domain.Sessions.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class ContactRequestRepository(ApplicationDbContext context) : IContactRequestRepository
{
    public async Task<ContactRequestEntity?> GetByIdAsync(ContactRequestId id, CancellationToken ct = default)
        => await context.Set<ContactRequestEntity>()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IEnumerable<ContactRequestEntity>> GetAllAsync(CancellationToken ct = default)
        => await context.Set<ContactRequestEntity>()
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.Created)
            .ToListAsync(ct);

    public async Task AddAsync(ContactRequestEntity entity, CancellationToken ct = default)
        => await context.Set<ContactRequestEntity>().AddAsync(entity, ct);

    public void Update(ContactRequestEntity entity)
        => context.Set<ContactRequestEntity>().Update(entity);

    public void Delete(ContactRequestEntity entity)
    {
    }

    public async Task<IEnumerable<ContactRequestEntity>> GetByEmailAsync(string email, CancellationToken ct = default)
        => await context.Set<ContactRequestEntity>()
            .Where(x => x.Email.Value == email && !x.IsDeleted)
            .ToListAsync(ct);
}