using Application.Abstractions.Authentication;
using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using Domain.Users.Entities;
using Domain.Users.Enums;
using MediatR;

namespace Application.Users.Commands.Create.User;

public sealed class RegisterQuickHandler(
    IUnitOfWork unitOfWork,
    IAuthService authService)
    : IRequestHandler<RegisterQuickCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterQuickCommand request, CancellationToken ct)
    {
        var identityResult = await authService.RegisterIdentityAsync(
            request.Email,
            request.Password,
            request.Role,
            ct);

        if (identityResult.IsFailure)
            return identityResult;

        var userRole = request.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase)
            ? UserRole.Admin
            : UserRole.Member;

        var user = UserEntity.Register(
            UserId.Create(identityResult.Value).Value,
            Email.Create(request.Email).Value,
            userRole);

        await unitOfWork.Users.AddAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(user.Id.Value);
    }
}