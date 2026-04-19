using Application.Sessions.Output;
using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using MediatR;

namespace Application.Booking;

public sealed class GetMyBookingsHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetMyBookingsQuery, Result<List<SessionResponse>>>
{
    public async Task<Result<List<SessionResponse>>> Handle(GetMyBookingsQuery request, CancellationToken ct)
    {
        var userId = UserId.Create(request.UserId).Value;
        var utcNow = DateTime.UtcNow;

        var sessions = await unitOfWork.Sessions.GetBookedSessionsForUserAsync(userId, utcNow, ct);

        var dtos = sessions
            .Select(s => new SessionResponse(
                s.Id.Value,
                s.Title.Value,
                s.Description.Value,
                s.Instructor.Value,
                s.Category.ToString(),
                s.Schedule.StartTime,
                s.Schedule.EndTime,
                s.MaxCapacity.Value,
                true
            )).ToList();

        return Result.Success(dtos);
    }
}