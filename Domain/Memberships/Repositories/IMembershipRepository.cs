using Domain.Common.Abstractions;
using Domain.Memberships.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Memberships.Repository;

public interface IMembershipRepository : IRepositoryBase<MembershipEntity>
{
    Task<MembershipEntity?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
}
