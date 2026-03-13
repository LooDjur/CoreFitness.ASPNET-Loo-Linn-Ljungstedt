using Domain.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Bookings.ValueObjects;

public record BookingSessionId
{
    public Guid Value { get; }
    public BookingSessionId(Guid value)
    {
        if (value == Guid.Empty)
            throw new DomainException("Session ID cannot be empty.");
        Value = value;
    }
    public static implicit operator Guid(BookingSessionId id) => id.Value;
}
