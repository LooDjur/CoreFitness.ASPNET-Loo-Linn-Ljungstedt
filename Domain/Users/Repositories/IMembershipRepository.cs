using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using Domain.Users.Entities;

namespace Domain.Users.Repositories;

public interface IMembershipRepository : IRepositoryBase<MembershipEntity, MemberId>
{
    Task<MembershipEntity?> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);
}
