using Domain.Common;
using Domain.Sessions.Enums;
using Domain.Sessions.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Sessions.Commands.Update;

public sealed record UpdateSessionCommand(
    Guid Id,
    string Title,
    string Description,
    string Instructor,
    SessionCategory Category,
    DateTime StartTime,
    DateTime EndTime,
    int MaxCapacity
) : IRequest<Result>;