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
        var sessionResult = SessionEntity.Create(
            new Title(request.Title),
            new Instructor(request.Instructor),
            request.Category,
            new TimeSlot(request.StartTime, request.EndTime),
            new Capacity(request.MaxCapacity),
            new Description(request.Description));

        if (sessionResult.IsFailure) return sessionResult.Error;

        var session = sessionResult.Value;

        await unitOfWork.Sessions.AddAsync(session, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return session.Id;
    }
}
