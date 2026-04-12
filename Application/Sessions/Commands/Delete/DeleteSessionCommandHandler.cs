using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using MediatR;

namespace Application.Sessions.Commands.Delete;

public sealed class DeleteSessionCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteSessionCommand, Result>
{
    public async Task<Result> Handle(DeleteSessionCommand request, CancellationToken ct)
    {
        var sessionIdResult = SessionId.Create(request.Id);

        if (sessionIdResult.IsFailure)
            return Result.Failure(sessionIdResult.Error);

        var session = await unitOfWork.Sessions.GetByIdAsync(sessionIdResult.Value, ct);

        if (session is null)
            return Result.Failure(DomainErrors.Session.NotFound);

        var deleteResult = session.Delete(request.UtcNow);

        if (deleteResult.IsFailure)
            return deleteResult;

        unitOfWork.Sessions.Update(session);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}