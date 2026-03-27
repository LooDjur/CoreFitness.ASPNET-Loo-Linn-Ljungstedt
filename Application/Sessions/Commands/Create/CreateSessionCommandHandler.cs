using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Sessions.Entities;
using Domain.Sessions.ValueObjects;
using MediatR;

namespace Application.Sessions.Commands.Create;

public sealed class CreateSessionCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateSessionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateSessionCommand request, CancellationToken ct)
    {
        var titleRes = Title.Create(request.Title);
        var descriptionRes = Description.Create(request.Description);
        var instructorRes = Instructor.Create(request.Instructor);
        var capacityRes = Capacity.Create(request.MaxCapacity);
        var timeRes = TimeSlot.Create(request.StartTime, request.EndTime);

        var validationResult = Result.FirstFailureOrSuccess(
            titleRes,
            descriptionRes,
            instructorRes,
            capacityRes,
            timeRes);

        if (validationResult.IsFailure)
            return Result.Failure<Guid>(validationResult.Error);

        var sessionResult = SessionEntity.Create(
            titleRes.Value,
            descriptionRes.Value,
            instructorRes.Value,
            request.Category,
            timeRes.Value,
            capacityRes.Value);

        if (sessionResult.IsFailure)
            return Result.Failure<Guid>(sessionResult.Error);

        var session = sessionResult.Value;

        await unitOfWork.Sessions.AddAsync(session, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(session.Id.Value);
    }
}