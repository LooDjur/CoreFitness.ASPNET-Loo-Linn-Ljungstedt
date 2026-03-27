using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using Domain.Users.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Users.Repositories;

public interface IUserRepository : IRepositoryBase<UserEntity, UserId>
{
    Task<UserEntity?> GetUserWithMembershipAsync(UserId id, CancellationToken ct);
    Task<UserEntity?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(Email email, CancellationToken ct);
}