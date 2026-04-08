using Application.Users.Output;
using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using Domain.Users.Enums;
using MediatR;

namespace Application.Users.Queries;

public sealed class GetUserProfileHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetUserProfileQuery, Result<UserResponse>>
{
    public async Task<Result<UserResponse>> Handle(GetUserProfileQuery request, CancellationToken ct)
    {
        var userId = UserId.Create(request.UserId).Value;

        var user = await unitOfWork.Users.GetUserWithMembershipAsync(userId, ct);

        if (user is null)
            return Result.Failure<UserResponse>(DomainErrors.User.NotFound);

        var response = new UserResponse(
            user.Id.Value,
            user.Membership?.Id.Value,
            user.Email.Value,
            user.FirstName?.Value,
            user.LastName?.Value,
            user.Phone?.Value,
            user.ProfileImageUrl,
            user.Membership?.Type.ToString() ?? "No active plan",
            user.Membership?.ExpiryDate,
            user.Membership?.IsEligibleToBook ?? false 
        );

        return Result.Success(response);
    }
}