using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using Domain.Users.Enums;
using MediatR;

namespace Application.Users.Commands.Create.Membership;

public sealed class CompleteOnboardingHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CompleteOnboardingCommand, Result>
{
    public async Task<Result> Handle(CompleteOnboardingCommand request, CancellationToken ct)
    {
        var user = await unitOfWork.Users.GetUserWithMembershipAsync(
            UserId.Create(request.UserId).Value, ct);

        if (user is null)
            return Result.Failure(DomainErrors.User.NotFound);

        user.CompleteProfile(
            FirstName.Create(request.FirstName).Value,
            LastName.Create(request.LastName).Value,
            PhoneNumber.Create(request.Phone).Value);

        if (!Enum.TryParse<MembershipType>(request.PlanType, true, out var type))
        {
            type = MembershipType.Standard;
        }

        var membershipResult = user.StartMembership(type);
        if (membershipResult.IsFailure)
            return membershipResult;

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}