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
        var user = await unitOfWork.Users.GetByIdAsync(UserId.Create(request.UserId).Value, ct);
        if (user is null) return Result.Failure(DomainErrors.User.NotFound);

        var newEmail = Email.Create(request.Email).Value;

        if (user.Email.Value != request.Email)
        {
            if (!await unitOfWork.Users.IsEmailUniqueAsync(newEmail, ct))
                return Result.Failure(DomainErrors.User.EmailInvalid);
        }

        var identityResult = await authService.UpdateIdentityUserAsync(
            request.UserId,
            request.Email,
            request.FirstName,
            request.LastName,
            request.Phone,
            ct);

        if (identityResult.IsFailure) return identityResult;

        user.UpdateProfile(
            FirstName.Create(request.FirstName).Value,
            LastName.Create(request.LastName).Value,
            newEmail,
            PhoneNumber.Create(request.Phone).Value,
            request.ProfileImageUrl);

        unitOfWork.Users.Update(user);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
