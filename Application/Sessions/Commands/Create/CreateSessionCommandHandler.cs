using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Sessions;
using MediatR;

namespace Application.Sessions.Commands.Create;

public sealed class CreateSessionCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateSessionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateSessionCommand request, CancellationToken ct)
    {
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
            return Result.Failure<Guid>(validationResult.Error);

        var sessionResult = SessionEntity.Create(
            titleResult.Value,
            descriptionResult.Value,
            instructorResult.Value,
            request.Category,
            timeSlotResult.Value,
            capacityResult.Value,
            request.UtcNow);

        await unitOfWork.Sessions.AddAsync(sessionResult.Value, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(sessionResult.Value.Id.Value);
    }
}