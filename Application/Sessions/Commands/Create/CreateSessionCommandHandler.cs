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
        var sessionResult = SessionEntity.Create(
        Title.Create(request.Title).Value,
        Description.Create(request.Description).Value,
        Instructor.Create(request.Instructor).Value,
        request.Category,
        TimeSlot.Create(request.StartTime, request.EndTime).Value,
        Capacity.Create(request.MaxCapacity).Value);

        if (sessionResult.IsFailure)
            return Result.Failure<Guid>(sessionResult.Error);

        await unitOfWork.Sessions.AddAsync(sessionResult.Value, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(sessionResult.Value.Id.Value);
    }
}