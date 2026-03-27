using Domain.Common;
using Domain.Sessions.Enums;
using MediatR;

namespace Application.Sessions.Commands.Create;

public sealed record CreateSessionCommand(
    string Title,
    string Description,
    string Instructor,
    SessionCategory Category,
    DateTime StartTime,
    DateTime EndTime,
    int MaxCapacity
) : IRequest<Result<Guid>>;
