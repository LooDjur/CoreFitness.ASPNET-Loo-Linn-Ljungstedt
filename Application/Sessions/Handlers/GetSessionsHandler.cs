using Application.Sessions.DTOs;
using Application.Sessions.Queries;
using Domain.Common;
using Domain.Common.Abstractions;
using MediatR;

namespace Application.Sessions.Handlers;

public sealed class GetSessionsHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetSessionsQuery, Result<List<SessionDto>>>
{
    public async Task<Result<List<SessionDto>>> Handle(GetSessionsQuery request, CancellationToken cancellationToken)
    {
        var sessions = await unitOfWork.Sessions.GetAllAsync(cancellationToken);

        var dtos = sessions.Select(s => new SessionDto
        {
            Id = s.Id,
            Title = s.Title.Value,
            Description = s.Description.Value,
            Instructor = s.Instructor.Value,
            Category = s.Category.ToString(),
            StartTime = s.Schedule.StartTime,
            EndTime = s.Schedule.EndTime,
            MaxCapacity = s.MaxCapacity.Value
        }).ToList();

        return dtos;
    }
}