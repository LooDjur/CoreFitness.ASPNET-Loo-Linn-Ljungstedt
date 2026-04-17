using Domain.Common;
using MediatR;

namespace Application.Booking;

public sealed record CancelBookingCommand(Guid SessionId, Guid UserId) : IRequest<Result>;
