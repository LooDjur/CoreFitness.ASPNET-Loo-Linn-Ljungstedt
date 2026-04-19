using Application.Abstractions.Authentication;
using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using MediatR;

namespace Application.Users.Commands.Delete.User;

public sealed class DeleteUserCommandHandler(
    IUnitOfWork unitOfWork,
    IAuthService authService)
    : IRequestHandler<DeleteUserCommand, Result>
{
    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken ct)
    {
        var userIdResult = UserId.Create(request.UserId);
        if (userIdResult.IsFailure)
            return Result.Failure(userIdResult.Error);

        var user = await unitOfWork.Users.GetUserWithMembershipAsync(userIdResult.Value, ct);

        if (user is null)
            return Result.Failure(DomainErrors.User.NotFound);

        var canDeleteResult = user.CanBePermanentlyDeleted();
        if (canDeleteResult.IsFailure)
            return canDeleteResult;

        var identityResult = await authService.DeleteIdentityUserAsync(user.Id.Value, ct);
        if (identityResult.IsFailure)
            return identityResult;

        await unitOfWork.Bookings.DeleteAllByUserIdAsync(user.Id, ct);

        if (user.Membership != null)
        {
            unitOfWork.Users.DeleteMembership(user.Membership);
        }

        unitOfWork.Users.Delete(user);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}