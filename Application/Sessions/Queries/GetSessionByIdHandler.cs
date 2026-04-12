using Application.Sessions.Output;
using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using MediatR;

namespace Application.Sessions.Queries;

public sealed class GetSessionByIdHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetSessionByIdQuery, Result<SessionResponse>>
{
    public async Task<Result<SessionResponse>> Handle(GetSessionByIdQuery request, CancellationToken ct)
    {
        var sessionIdResult = SessionId.Create(request.Id);
        if (sessionIdResult.IsFailure)
            return Result.Failure<SessionResponse>(sessionIdResult.Error);

        var s = await unitOfWork.Sessions.GetByIdAsync(sessionIdResult.Value, ct);

        if (s is null || s.IsDeleted)
            return Result.Failure<SessionResponse>(DomainErrors.Session.NotFound);

        bool isBooked = false;
        if (request.UserId.HasValue)
        {
            var userId = UserId.Create(request.UserId.Value).Value;
            var user = await unitOfWork.Users.GetByIdAsync(userId, ct);

            if (user?.Membership != null)
            {
                isBooked = await unitOfWork.Bookings.IsAlreadyBookedAsync(
                    s.Id,
                    user.Membership.Id,
                    ct);
            }
        }

        return Result.Success(new SessionResponse(
            s.Id.Value,
            s.Title.Value,
            s.Description.Value,
            s.Instructor.Value,
            s.Category.ToString(),
            s.Schedule.StartTime,
            s.Schedule.EndTime,
            s.MaxCapacity.Value,
            isBooked
        ));
    }
}