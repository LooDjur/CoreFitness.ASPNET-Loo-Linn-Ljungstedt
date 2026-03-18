using Domain.Bookings.Repositories;
using Domain.Memberships.Repository;
using Domain.Sessions.Repositories;

namespace Domain.Common.Abstractions;

public interface IUnitOfWork : IDisposable
{
    ISessionRepository Sessions { get; }
    IBookingRepository Bookings { get; }
    IMembershipRepository Memberships { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
