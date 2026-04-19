using Application.Sessions.Output;
using Domain.Common;
using MediatR;

namespace Application.Booking;

public sealed record GetMyBookingsQuery(Guid UserId) : IRequest<Result<List<SessionResponse>>>;