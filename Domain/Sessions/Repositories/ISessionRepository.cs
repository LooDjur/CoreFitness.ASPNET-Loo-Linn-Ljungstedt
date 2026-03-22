using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using Domain.Sessions.Entities;
using Domain.Sessions.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Sessions.Repositories;

public interface ISessionRepository : IRepositoryBase<SessionEntity, SessionId>
{
    Task<IEnumerable<SessionEntity>> GetUpcomingSessionsAsync(CancellationToken ct = default);
    Task<IEnumerable<SessionEntity>> GetByCategoryAsync(SessionCategory category, CancellationToken ct = default);
}
