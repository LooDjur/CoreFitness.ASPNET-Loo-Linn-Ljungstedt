using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Sessions.Commands.Delete;

public sealed class DeleteSessionCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteSessionCommand, Result>
{
    public async Task<Result> Handle(DeleteSessionCommand request, CancellationToken ct)
    {
        var sessionId = SessionId.Create(request.Id).Value;

        var session = await unitOfWork.Sessions.GetByIdAsync(sessionId, ct);

        if (session is null)
            return Result.Failure(DomainErrors.Session.NotFound);

        var deleteResult = session.Delete();
        if (deleteResult.IsFailure)
            return deleteResult;

        unitOfWork.Sessions.Update(session);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}