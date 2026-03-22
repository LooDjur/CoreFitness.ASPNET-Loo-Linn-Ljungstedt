using Application.Sessions.Commands;
using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using Domain.Sessions.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Sessions.Handlers;

public sealed class UpdateSessionHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateSessionCommand, Result>
{
    public async Task<Result> Handle(UpdateSessionCommand request, CancellationToken ct)
    {
        var sessionIdResult = SessionId.Create(request.Id);
        if (sessionIdResult.IsFailure)
            return Result.Failure(sessionIdResult.Error);

        var session = await unitOfWork.Sessions.GetByIdAsync(sessionIdResult.Value, ct);

        if (session is null)
            return Result.Failure(DomainErrors.Session.NotFound);

        var titleResult = Title.Create(request.Title);
        var descriptionResult = Description.Create(request.Description);
        var instructorResult = Instructor.Create(request.Instructor);
        var capacityResult = Capacity.Create(request.MaxCapacity);
        var timeSlotResult = TimeSlot.Create(request.StartTime, request.EndTime);

        var firstFailure = Result.FirstFailureOrSuccess(
            titleResult,
            descriptionResult,
            instructorResult,
            capacityResult,
            timeSlotResult);

        if (firstFailure.IsFailure) return firstFailure;

        session.UpdateDetails(
            titleResult.Value,
            descriptionResult.Value,
            instructorResult.Value,
            request.Category,
            timeSlotResult.Value,
            capacityResult.Value);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}