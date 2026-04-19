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
        var userIdResult = UserId.Create(request.UserId);
        if (userIdResult.IsFailure)
            return Result.Failure<UserResponse>(userIdResult.Error);

        var user = await unitOfWork.Users.GetUserWithMembershipAsync(userIdResult.Value, ct);

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
            user.Membership?.IsEligibleToBook(request.UtcNow) ?? false
        );

        return Result.Success(response);
    }
}