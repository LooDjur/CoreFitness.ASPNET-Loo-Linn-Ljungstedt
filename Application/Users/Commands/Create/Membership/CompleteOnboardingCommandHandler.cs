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
        if (userIdResult.IsFailure)
            return Result.Failure(userIdResult.Error);

        var user = await unitOfWork.Users.GetWithMembershipIgnoringFiltersAsync(userIdResult.Value, ct);

        if (user is null)
            return Result.Failure(DomainErrors.User.NotFound);

        var firstNameResult = FirstName.Create(request.FirstName);
        var lastNameResult = LastName.Create(request.LastName);
        var phoneResult = PhoneNumber.Create(request.Phone);

        var validationResult = Result.FirstFailureOrSuccess(
            firstNameResult,
            lastNameResult,
            phoneResult);

        if (validationResult.IsFailure)
            return Result.Failure(validationResult.Error);

        user.CompleteProfile(
            firstNameResult.Value,
            lastNameResult.Value,
            phoneResult.Value,
            request.UtcNow);

        if (!Enum.TryParse<MembershipType>(request.PlanType, true, out var type))
        {
            type = MembershipType.Standard;
        }

        var membershipResult = user.StartMembership(type, DateTime.UtcNow);
        if (membershipResult.IsFailure)
            return membershipResult;

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}