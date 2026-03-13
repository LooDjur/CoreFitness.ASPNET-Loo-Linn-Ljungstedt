using Domain.Bookings.Repositories;
using Domain.Memberships.Repository;
using Domain.Sessions.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        ISessionRepository Sessions { get; }
        IBookingRepository Bookings { get; }
        IMembershipRepository Memberships { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
