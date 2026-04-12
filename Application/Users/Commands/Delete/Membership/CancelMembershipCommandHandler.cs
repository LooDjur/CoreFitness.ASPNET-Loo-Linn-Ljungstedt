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
        var userIdResult = UserId.Create(request.UserId);
        if (userIdResult.IsFailure) return Result.Failure(userIdResult.Error);

        var user = await unitOfWork.Users.GetUserWithMembershipAsync(userIdResult.Value, ct);

        if (user is null) return Result.Failure(DomainErrors.User.NotFound);
        if (user.Membership is null) return Result.Success();

        var bookings = await unitOfWork.Bookings.GetByMemberIdAsync(user.Membership.Id, ct);

        foreach (var booking in bookings)
        {
            booking.Delete(request.UtcNow);
        }

        var cancelResult = user.CancelMembership(request.UtcNow);
        if (cancelResult.IsFailure) return cancelResult;

        unitOfWork.Users.Update(user);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}