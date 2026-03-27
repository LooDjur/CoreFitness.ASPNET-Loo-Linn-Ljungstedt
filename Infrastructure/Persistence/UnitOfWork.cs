using Domain.Bookings.Repositories;
using Domain.Common.Abstractions;
using Domain.Sessions.Repositories;
using Domain.Users.Repositories;
using Infrastructure.Persistence.Context;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence;

public sealed class UnitOfWork(ApplicationDbContext context) : IUnitOfWork, IDisposable
{
    private IUserRepository? _users;
    private IMembershipRepository? _memberships;
    private ISessionRepository? _sessions;
    private IBookingRepository? _bookings;

    public IUserRepository Users => _users ??= new UserRepository(context);
    public IMembershipRepository Memberships => _memberships ??= new MembershipRepository(context);
    public ISessionRepository Sessions => _sessions ??= new SessionRepository(context);

    public IBookingRepository Bookings => _bookings ??= null!;

    public async Task<int> SaveChangesAsync(CancellationToken ct)
        => await context.SaveChangesAsync(ct);

    public void Dispose()
    {
        context.Dispose();
    }
}