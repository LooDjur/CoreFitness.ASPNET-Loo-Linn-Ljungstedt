using Domain.Common;
using MediatR;

namespace Application.Booking;

public sealed record CreateBookingCommand(
    Guid SessionId,
    Guid UserId,
    DateTime UtcNow
) : IRequest<Result<Guid>>;