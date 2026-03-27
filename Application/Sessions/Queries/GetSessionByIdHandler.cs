using Application.Sessions.Output;
using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Sessions.Queries;

public sealed class GetSessionByIdHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetSessionByIdQuery, Result<SessionOutput>>
{
    public async Task<Result<SessionOutput>> Handle(GetSessionByIdQuery request, CancellationToken ct)
    {
        var sessionIdResult = SessionId.Create(request.Id);
        if (sessionIdResult.IsFailure)
            return Result.Failure<SessionOutput>(sessionIdResult.Error);

        var s = await unitOfWork.Sessions.GetByIdAsync(sessionIdResult.Value, ct);

        if (s is null || s.IsDeleted)
            return Result.Failure<SessionOutput>(DomainErrors.Session.NotFound);

        return Result.Success(new SessionOutput(
            s.Id.Value,
            s.Title.Value,
            s.Description.Value,
            s.Instructor.Value,
            s.Category.ToString(),
            s.Schedule.StartTime,
            s.Schedule.EndTime,
            s.MaxCapacity.Value
        ));
    }
}
