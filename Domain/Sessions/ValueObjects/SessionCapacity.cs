using Domain.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Sessions.ValueObjects;

public record SessionCapacity
{
    public int Value { get; }
    public SessionCapacity(int value)
    {
        if (value < 10) throw new DomainException("Capacity must be at least 10.");
        if (value > 40) throw new DomainException("Capacity cannot exceed 40.");
        Value = value;
    }
    public static implicit operator int(SessionCapacity capacity) => capacity.Value;
}