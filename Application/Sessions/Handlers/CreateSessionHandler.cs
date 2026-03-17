using Application.Sessions.Commands;
using Domain.Abstractions;
using Domain.Common;
using Domain.Sessions.Entities;
using Domain.Sessions.ValueObjects;
using MediatR;

namespace Application.Sessions.Handlers;

public sealed class CreateSessionHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateSessionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
    {
        var titleRes = Title.Create(request.Title);
        var instructorRes = Instructor.Create(request.Instructor);
        var timeRes = TimeSlot.Create(request.StartTime, request.EndTime);
        var capacityRes = Capacity.Create(request.MaxCapacity);
        var descriptionRes = Description.Create(request.Description);

        if (Result.FirstFailureOrSuccess(titleRes, instructorRes, timeRes, capacityRes, descriptionRes)
            is var failure && failure.IsFailure)
        {
            return Result.Failure<Guid>(failure.Error);
        }

        var sessionResult = SessionEntity.Create(
            titleRes.Value,
            instructorRes.Value,
            request.Category,
            timeRes.Value,
            capacityRes.Value,
            descriptionRes.Value);

        if (sessionResult.IsFailure)
            return Result.Failure<Guid>(sessionResult.Error);

        var session = sessionResult.Value;

        await unitOfWork.Sessions.AddAsync(session, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return session.Id;
    }
}