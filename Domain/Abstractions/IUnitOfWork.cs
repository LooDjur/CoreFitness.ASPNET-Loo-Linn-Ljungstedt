using Domain.Courses.Repository;
using Domain.Memberships.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        ICourseRepository Courses { get; }
        IBookingRepository Bookings { get; }
        IMembershipRepository Memberships { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
