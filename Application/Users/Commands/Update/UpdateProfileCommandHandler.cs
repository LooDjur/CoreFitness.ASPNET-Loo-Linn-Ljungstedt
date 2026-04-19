using Application.Abstractions.Authentication;
using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using MediatR;

namespace Application.Users.Commands.Update;

public sealed class UpdateProfileHandler(
    IUnitOfWork unitOfWork,
    IAuthService authService)
    : IRequestHandler<UpdateProfileCommand, Result>
{
    public async Task<Result> Handle(UpdateProfileCommand request, CancellationToken ct)
    {
        var userIdResult = UserId.Create(request.UserId);
        if (userIdResult.IsFailure)
            return Result.Failure(userIdResult.Error);

        var user = await unitOfWork.Users.GetByIdAsync(userIdResult.Value, ct);
        if (user is null)
            return Result.Failure(DomainErrors.User.NotFound);

        var firstNameResult = FirstName.Create(request.FirstName);
        var lastNameResult = LastName.Create(request.LastName);
        var emailResult = Email.Create(request.Email);
        var phoneResult = PhoneNumber.Create(request.Phone);

        var validationResult = Result.FirstFailureOrSuccess(
            firstNameResult,
            lastNameResult,
            emailResult,
            phoneResult);

        if (validationResult.IsFailure)
            return Result.Failure(validationResult.Error);

        if (user.Email.Value != request.Email)
        {
            if (!await unitOfWork.Users.IsEmailUniqueAsync(emailResult.Value, ct))
                return Result.Failure(DomainErrors.User.EmailInvalid);
        }

        var identityResult = await authService.UpdateIdentityUserAsync(
            request.UserId,
            request.Email,
            request.FirstName,
            request.LastName,
            request.Phone,
            ct);

        if (identityResult.IsFailure)
            return identityResult;

        user.UpdateProfile(
            firstNameResult.Value,
            lastNameResult.Value,
            emailResult.Value,
            phoneResult.Value,
            request.ProfileImageUrl,
            request.UtcNow);

        unitOfWork.Users.Update(user);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
