using Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Booking;

public sealed record CancelBookingCommand(Guid SessionId, Guid UserId) : IRequest<Result>;
