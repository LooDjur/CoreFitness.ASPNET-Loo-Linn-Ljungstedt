using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using Domain.Sessions;
using MediatR;

namespace Application.Sessions.Commands.Update;

public sealed class UpdateSessionCommandHandler(IUnitOfWork unitOfWork)
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
        var timeSlotResult = TimeSlot.Create(request.StartTime, request.EndTime);
        var capacityResult = Capacity.Create(request.MaxCapacity);

        var validationResult = Result.FirstFailureOrSuccess(
            titleResult,
            descriptionResult,
            instructorResult,
            timeSlotResult,
            capacityResult);

        if (validationResult.IsFailure)
            return Result.Failure(validationResult.Error);

        var updateResult = session.UpdateDetails(
            titleResult.Value,
            descriptionResult.Value,
            instructorResult.Value,
            request.Category,
            timeSlotResult.Value,
            capacityResult.Value,
            request.UtcNow);

        if (updateResult.IsFailure)
            return updateResult;

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}