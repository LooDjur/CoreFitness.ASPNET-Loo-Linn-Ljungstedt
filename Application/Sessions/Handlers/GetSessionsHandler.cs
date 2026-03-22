using Application.Sessions.Output;
using Application.Sessions.Queries;
using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using MediatR;

namespace Application.Sessions.Handlers;

public sealed class GetSessionsHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetSessionsQuery, Result<List<SessionOutput>>>
{
    public async Task<Result<List<SessionOutput>>> Handle(GetSessionsQuery request, CancellationToken ct)
    {
        var sessions = await unitOfWork.Sessions.GetAllAsync(ct);

        var dtos = sessions
            .Where(s => !s.IsDeleted)
            .Select(s => new SessionOutput(
                s.Id.Value,
                s.Title.Value,
                s.Description.Value,
                s.Instructor.Value,
                s.Category.ToString(),
                s.Schedule.StartTime,
                s.Schedule.EndTime,
                s.MaxCapacity.Value
            )).ToList();

        return Result.Success(dtos);
    }
}