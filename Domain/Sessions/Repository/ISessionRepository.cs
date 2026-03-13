using Domain.Abstractions;
using Domain.Sessions.Entities;
using Domain.Sessions.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Sessions.Repository;

public interface ISessionRepository : IRepositoryBase<SessionEntity>
{
    Task<IEnumerable<SessionEntity>> GetUpcomingSessionsAsync(CancellationToken ct = default);
    Task<IEnumerable<SessionEntity>> GetByCategoryAsync(SessionCategory category, CancellationToken ct = default);
}
