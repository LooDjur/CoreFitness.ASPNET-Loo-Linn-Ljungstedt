using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using Domain.Users.Entities;
using MediatR;

namespace Application.Users.Commands.Create.User;

public sealed class RegisterQuickHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<RegisterQuickCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterQuickCommand request, CancellationToken ct)
    {
        var emailRes = Email.Create(request.Email);
        if (emailRes.IsFailure) return Result.Failure<Guid>(emailRes.Error);

        var existing = await unitOfWork.Users.GetByEmailAsync(emailRes.Value, ct);
        if (existing is not null) return Result.Success(existing.Id.Value);

        var user = UserEntity.Register(emailRes.Value);
        await unitOfWork.Users.AddAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(user.Id.Value);
    }
}
