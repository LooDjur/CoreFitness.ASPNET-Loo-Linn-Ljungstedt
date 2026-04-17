using Domain.Bookings.Repositories;
using Domain.ContactReq.Repositories;
using Domain.Sessions;
using Domain.Users.Repositories;

namespace Domain.Common.Abstractions;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ISessionRepository Sessions { get; }
    IBookingRepository Bookings { get; }
    IContactRequestRepository ContactRequests { get; }


    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
