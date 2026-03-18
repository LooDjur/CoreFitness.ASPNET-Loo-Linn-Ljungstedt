using Domain.Bookings.Repositories;
using Domain.Common.Abstractions;
using Domain.Memberships.Repository;
using Domain.Sessions.Repositories;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence;

public sealed class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private ISessionRepository? _sessions;
    private IBookingRepository? _bookings;
    private IMembershipRepository? _memberships;

    public ISessionRepository Sessions => _sessions ??= new SessionRepository(context);
    public IBookingRepository Bookings => _bookings ??= null!;
    public IMembershipRepository Memberships => _memberships ??= null!;

    public async Task<int> SaveChangesAsync(CancellationToken ct)
        => await context.SaveChangesAsync(ct);

    public void Dispose() => context.Dispose();
}