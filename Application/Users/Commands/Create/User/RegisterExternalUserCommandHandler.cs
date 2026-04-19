using Application.Abstractions.Authentication;
using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using Domain.Users.Entities;
using Domain.Users.Enums;
using MediatR;

namespace Application.Users.Commands.Create.User;

public sealed class RegisterExternalUserCommandHandler(
    IUnitOfWork unitOfWork,
    IAuthService authService)
    : IRequestHandler<RegisterExternalUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterExternalUserCommand request, CancellationToken ct)
    {
        var identityResult = await authService.RegisterExternalIdentityAsync(
            request.Email,
            request.Provider,
            request.ProviderKey,
            ct);

        if (identityResult.IsFailure)
            return Result.Failure<Guid>(identityResult.Error);

        var userId = identityResult.Value;
        var existingUser = await unitOfWork.Users.GetByIdAsync(UserId.Create(userId).Value, ct);

        if (existingUser is not null)
        {
            return Result.Success(existingUser.Id.Value);
        }

        var user = UserEntity.Register(
            UserId.Create(userId).Value,
            Email.Create(request.Email).Value,
            DateTime.UtcNow,
            UserRole.Member);

        await unitOfWork.Users.AddAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(user.Id.Value);
    }
}
