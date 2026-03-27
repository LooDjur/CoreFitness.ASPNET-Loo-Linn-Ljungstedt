using Application.Users.Output;
using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Users.Queries;

public sealed class GetUserProfileHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetUserProfileQuery, Result<UserResponse>>
{
    public async Task<Result<UserResponse>> Handle(GetUserProfileQuery request, CancellationToken ct)
    {
        var userId = UserId.Create(request.UserId).Value;

        var user = await unitOfWork.Users.GetByIdAsync(userId, ct);
        if (user is null) return Result.Failure<UserResponse>(DomainErrors.User.NotFound);

        var membership = await unitOfWork.Memberships.GetByUserIdAsync(userId, ct);

        var response = new UserResponse(
            user.Id.Value,
            user.Email.Value,
            user.FirstName?.Value,
            user.LastName?.Value,
            user.Phone?.Value,
            user.ProfileImageUrl,
            membership?.Type.ToString(),
            membership?.IsEligibleToBook ?? false
        );

        return Result.Success(response);
    }
}
