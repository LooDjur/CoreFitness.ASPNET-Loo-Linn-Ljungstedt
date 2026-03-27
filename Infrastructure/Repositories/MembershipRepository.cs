using Domain.Common.ValueObjects.Shared;
using Domain.Users.Entities;
using Domain.Users.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories;

public sealed class MembershipRepository(ApplicationDbContext context) : IMembershipRepository
{
    public async Task<MembershipEntity?> GetByUserIdAsync(UserId userId, CancellationToken ct)
    {
        return await context.Memberships
            .FirstOrDefaultAsync(m => m.UserId == userId, ct);
    }

    public async Task AddAsync(MembershipEntity membership, CancellationToken ct)
    {
        await context.Memberships.AddAsync(membership, ct);
    }

    public async Task<MembershipEntity?> GetByIdAsync(MemberId id, CancellationToken ct)
    {
        return await context.Memberships.FirstOrDefaultAsync(m => m.Id == id, ct);
    }

    public async Task<IEnumerable<MembershipEntity>> GetAllAsync(CancellationToken ct)
    {
        return await context.Memberships.ToListAsync(ct);
    }

    public void Update(MembershipEntity membership)
    {
        context.Memberships.Update(membership);
    }

    public void Delete(MembershipEntity membership)
    {
        context.Memberships.Remove(membership);
    }
}
