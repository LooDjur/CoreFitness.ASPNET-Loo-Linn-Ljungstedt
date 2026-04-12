using Application.Sessions.Output;
using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using MediatR;

namespace Application.Sessions.Queries;

public sealed class GetSessionsHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetSessionsQuery, Result<List<SessionResponse>>>
{
    public async Task<Result<List<SessionResponse>>> Handle(GetSessionsQuery request, CancellationToken ct)
    {
        var sessions = await unitOfWork.Sessions.GetAllAsync(ct);
        var bookedSessionIds = new HashSet<Guid>();

        if (request.UserId.HasValue)
        {
            var userIdResult = UserId.Create(request.UserId.Value);
            if (userIdResult.IsSuccess)
            {
                var user = await unitOfWork.Users.GetByIdAsync(userIdResult.Value, ct);

                if (user?.Membership != null)
                {
                    var userBookings = await unitOfWork.Bookings.GetByMemberIdAsync(user.Membership.Id, ct);
                    bookedSessionIds = [.. userBookings.Select(b => b.SessionId.Value)];
                }
            }
        }

        var dtos = sessions
            .Where(s => !s.IsDeleted)
            .Select(s => new SessionResponse(
                s.Id.Value,
                s.Title.Value,
                s.Description.Value,
                s.Instructor.Value,
                s.Category.ToString(),
                s.Schedule.StartTime,
                s.Schedule.EndTime,
                s.MaxCapacity.Value,
                bookedSessionIds.Contains(s.Id.Value)
            )).ToList();

        return Result.Success(dtos);
    }
}