using Domain.Abstractions;
using Domain.Bookings.Entity;
using Domain.Sessions.Entities;
using Domain.Sessions.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Bookings.Repositories;

public interface IBookingRepository : IRepositoryBase<BookingEntity>
{
    Task<IEnumerable<BookingEntity>> GetUpcomingSessionsAsync(CancellationToken ct = default);
    Task<IEnumerable<BookingEntity>> GetByCategoryAsync(SessionCategory category, CancellationToken ct = default);
}
