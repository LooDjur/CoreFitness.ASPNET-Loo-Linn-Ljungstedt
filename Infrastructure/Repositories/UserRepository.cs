using Domain.Common.ValueObjects.Shared;
using Domain.Users.Entities;
using Domain.Users.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public async Task<UserEntity?> GetByIdAsync(UserId id, CancellationToken ct)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<UserEntity?> GetByEmailAsync(Email email, CancellationToken ct)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    public async Task<bool> IsEmailUniqueAsync(Email email, CancellationToken ct)
    {
        return !await context.Users.AnyAsync(u => u.Email == email, ct);
    }

    public async Task AddAsync(UserEntity user, CancellationToken ct)
    {
        await context.Users.AddAsync(user, ct);
    }

    public async Task<IEnumerable<UserEntity>> GetAllAsync(CancellationToken ct)
    {
        return await context.Users.ToListAsync(ct);
    }

    public void Update(UserEntity user)
    {
        context.Users.Update(user);
    }

    public void Delete(UserEntity user)
    {
        context.Users.Remove(user);
    }
    public async Task<UserEntity?> GetUserWithMembershipAsync(UserId id, CancellationToken ct)
    {
        return await context.Users
            .Include(u => u.Membership)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }
}