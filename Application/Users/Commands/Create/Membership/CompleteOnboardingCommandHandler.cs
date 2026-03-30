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
        var userIdResult = UserId.Create(request.UserId);
        if (userIdResult.IsFailure) return Result.Failure(userIdResult.Error);

        var user = await unitOfWork.Users.GetByIdAsync(userIdResult.Value, ct);
        if (user is null) return Result.Failure(DomainErrors.User.NotFound);

        var fnRes = FirstName.Create(request.FirstName);
        var lnRes = LastName.Create(request.LastName);
        var nameValidation = Result.FirstFailureOrSuccess(fnRes, lnRes);
        if (nameValidation.IsFailure) return nameValidation;

        PhoneNumber? phone = null;
        if (!string.IsNullOrWhiteSpace(request.Phone))
        {
            var phoneRes = PhoneNumber.Create(request.Phone);
            if (phoneRes.IsFailure) return Result.Failure(phoneRes.Error);
            phone = phoneRes.Value;
        }

        user.CompleteProfile(fnRes.Value, lnRes.Value, phone);

        if (!Enum.TryParse<MembershipType>(request.PlanType, true, out var type))
        {
            type = MembershipType.Standard;
        }

        var membershipResult = user.StartMembership(type);
        if (membershipResult.IsFailure) return membershipResult;

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}