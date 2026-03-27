using Domain.Bookings.Repositories;
using Domain.Sessions.Repositories;
using Domain.Users.Repositories;

namespace Domain.Common.Abstractions;

public interface IUnitOfWork : IDisposable
{
    ISessionRepository Sessions { get; }
    IBookingRepository Bookings { get; }
    IUserRepository Users { get; }
    IMembershipRepository Memberships { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
