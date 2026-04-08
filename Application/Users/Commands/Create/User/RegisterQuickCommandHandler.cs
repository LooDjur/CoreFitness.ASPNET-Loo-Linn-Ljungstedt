using Application.Abstractions.Authentication;
using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using Domain.Users.Entities;
using MediatR;

namespace Application.Users.Commands.Create.User;

public sealed class RegisterQuickHandler(
    IUnitOfWork unitOfWork,
    IAuthService authService)
    : IRequestHandler<RegisterQuickCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterQuickCommand request, CancellationToken ct)
    {
        var emailRes = Email.Create(request.Email);
        if (emailRes.IsFailure)
            return Result.Failure<Guid>(emailRes.Error);

        var identityResult = await authService.RegisterIdentityAsync(request.Email, request.Password, request.Role, ct);
        if (identityResult.IsFailure)
            return identityResult;

        var userId = UserId.Create(identityResult.Value);
        var user = UserEntity.Register(userId.Value, emailRes.Value);

        try
        {
            await unitOfWork.Users.AddAsync(user, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception)
        {
            return Result.Failure<Guid>(Error.Failure("Db.SaveError", "Kunde inte spara användaren i databasen."));
        }

        return Result.Success(user.Id.Value);
    }
}
