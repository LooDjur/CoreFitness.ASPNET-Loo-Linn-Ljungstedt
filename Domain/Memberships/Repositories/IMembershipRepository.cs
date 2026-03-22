using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using Domain.Memberships.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Memberships.Repositories;

public interface IMembershipRepository : IRepositoryBase<MembershipEntity, MemberId>
{
    Task<MembershipEntity?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
}
