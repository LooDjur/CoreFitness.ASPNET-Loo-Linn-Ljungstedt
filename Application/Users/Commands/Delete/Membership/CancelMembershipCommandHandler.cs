using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using MediatR;

namespace Application.Users.Commands.Delete.Membership;

public sealed class CancelMembershipHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CancelMembershipCommand, Result>
{
    public async Task<Result> Handle(CancelMembershipCommand request, CancellationToken ct)
    {
        var user = await unitOfWork.Users.GetUserWithMembershipAsync(
            UserId.Create(request.UserId).Value, ct);

        if (user is null)
            return Result.Failure(DomainErrors.User.NotFound);

        user.CancelMembership();
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}