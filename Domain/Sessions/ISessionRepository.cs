using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Sessions;

public interface ISessionRepository : IRepositoryBase<SessionEntity, SessionId>
{
    Task<IEnumerable<SessionEntity>> GetUpcomingSessionsAsync(DateTime utcNow, CancellationToken ct = default);
    Task<IEnumerable<SessionEntity>> GetByCategoryAsync(SessionCategory category, CancellationToken ct = default);
}
