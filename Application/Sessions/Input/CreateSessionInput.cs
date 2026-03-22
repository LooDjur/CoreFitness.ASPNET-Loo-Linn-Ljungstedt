using Domain.Sessions.Enums;
using Domain.Sessions.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Sessions.Input;

public record CreateSessionInput(
    string Title,
    string Description,
    string Instructor,
    SessionCategory Category,
    DateTime StartTime,
    DateTime EndTime,
    Capacity MaxCapacity
);
