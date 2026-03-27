using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Users.Commands.Delete;

public sealed class DeleteUserCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteUserCommand, Result>
{
    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken ct)
    {
        var userId = UserId.Create(request.UserId).Value;

        var user = await unitOfWork.Users.GetByIdAsync(userId, ct);
        if (user is null) return Result.Failure(DomainErrors.User.NotFound);

        var membership = await unitOfWork.Memberships.GetByUserIdAsync(userId, ct);
        if (membership is not null)
        {
            unitOfWork.Memberships.Delete(membership);
        }

        unitOfWork.Users.Delete(user);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}