using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using Domain.Sessions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Sessions.Commands.Update;

public sealed class UpdateSessionCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateSessionCommand, Result>
{
    public async Task<Result> Handle(UpdateSessionCommand request, CancellationToken ct)
    {
        var session = await unitOfWork.Sessions.GetByIdAsync(
            SessionId.Create(request.Id).Value, ct);

        if (session is null)
            return Result.Failure(DomainErrors.Session.NotFound);

        session.UpdateDetails(
            Title.Create(request.Title).Value,
            Description.Create(request.Description).Value,
            Instructor.Create(request.Instructor).Value,
            request.Category,
            TimeSlot.Create(request.StartTime, request.EndTime).Value,
            Capacity.Create(request.MaxCapacity).Value);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}