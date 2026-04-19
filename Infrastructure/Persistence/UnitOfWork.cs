using Domain.Bookings.Repositories;
using Domain.Common.Abstractions;
using Domain.ContactReq.Repositories;
using Domain.Sessions;
using Domain.Users.Repositories;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories;

namespace Infrastructure.Persistence;

public sealed class UnitOfWork(ApplicationDbContext context) : IUnitOfWork, IDisposable
{
    private IUserRepository? _users;
    private ISessionRepository? _sessions;
    private IBookingRepository? _bookings;
    private IContactRequestRepository? _contactRequests;

    public IUserRepository Users => _users ??= new UserRepository(context);
    public ISessionRepository Sessions => _sessions ??= new SessionRepository(context);
    public IBookingRepository Bookings => _bookings ??= new BookingRepository(context);
    public IContactRequestRepository ContactRequests => _contactRequests ??= new ContactRequestRepository(context);
    public async Task<int> SaveChangesAsync(CancellationToken ct)
        => await context.SaveChangesAsync(ct);

    public void Dispose()
    {
        context.Dispose();
    }
}