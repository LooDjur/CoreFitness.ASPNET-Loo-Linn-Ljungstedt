using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using MediatR;

namespace Application.Users.Commands.Update;

public sealed class UpdateProfileHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateProfileCommand, Result>
{
    public async Task<Result> Handle(UpdateProfileCommand request, CancellationToken ct)
    {
        var userId = UserId.Create(request.UserId).Value;
        var user = await unitOfWork.Users.GetByIdAsync(userId, ct);
        if (user is null) return Result.Failure(DomainErrors.User.NotFound);

        var emailRes = Email.Create(request.Email);
        var fnRes = FirstName.Create(request.FirstName);
        var lnRes = LastName.Create(request.LastName);

        var validation = Result.FirstFailureOrSuccess(emailRes, fnRes, lnRes);
        if (validation.IsFailure) return validation;

        if (user.Email != emailRes.Value && !await unitOfWork.Users.IsEmailUniqueAsync(emailRes.Value, ct))
            return Result.Failure(DomainErrors.User.EmailInvalid);

        PhoneNumber? phone = !string.IsNullOrWhiteSpace(request.Phone)
            ? PhoneNumber.Create(request.Phone).Value : null;

        user.UpdateProfile(fnRes.Value, lnRes.Value, emailRes.Value, phone, request.ProfileImageUrl);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
