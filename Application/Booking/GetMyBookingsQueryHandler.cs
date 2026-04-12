using Application.Sessions.Output;
using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Booking;

public sealed class GetMyBookingsHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetMyBookingsQuery, Result<List<SessionResponse>>>
{
    public async Task<Result<List<SessionResponse>>> Handle(GetMyBookingsQuery request, CancellationToken ct)
    {
        var user = await unitOfWork.Users.GetByIdAsync(UserId.Create(request.UserId).Value, ct);

        if (user?.Membership == null)
        {
            return Result.Success(new List<SessionResponse>());
        }

        var bookings = await unitOfWork.Bookings.GetByMemberIdAsync(user.Membership.Id, ct);

        if (!bookings.Any())
            return Result.Success(new List<SessionResponse>());

        var bookedSessionIds = bookings.Select(b => b.SessionId.Value).ToHashSet();

        var allSessions = await unitOfWork.Sessions.GetAllAsync(ct);

        var dtos = allSessions
            .Where(s => bookedSessionIds.Contains(s.Id.Value) && !s.IsDeleted)
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