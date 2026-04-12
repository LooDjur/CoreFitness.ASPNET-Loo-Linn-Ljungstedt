using Domain.Common;
using Domain.Sessions;
using MediatR;

namespace Application.Sessions.Commands.Update;

public sealed record UpdateSessionCommand(
    Guid Id,
    string Title,
    string Description,
    string Instructor,
    SessionCategory Category,
    DateTime StartTime,
    DateTime EndTime,
    int MaxCapacity,
    DateTime UtcNow
) : IRequest<Result>;