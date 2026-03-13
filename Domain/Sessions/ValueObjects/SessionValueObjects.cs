using Domain.Common;
using Domain.Common.Exceptions;
using Domain.Common.ValueObjects.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Sessions.ValueObjects;

public record Title(string Value) : StringValueObject(Value, 3, 20);
public record Instructor(string Value) : StringValueObject(Value, 3, 100);
public record Description(string Value) : StringValueObject(Value, 0, 500);
public record Capacity(int Value) : RangeValueObject(Value, 10, 40, DomainErrors.Session.InvalidCapacity);
public record TimeSlot
{
    public DateTime StartTime { get; }
    public DateTime EndTime { get; }

    public TimeSlot(DateTime start, DateTime end)
    {
        if (end <= start)
            throw new DomainException(DomainErrors.Session.InvalidDate);

        if (start < DateTime.UtcNow)
            throw new DomainException(DomainErrors.Session.InvalidDate);

        StartTime = start;
        EndTime = end;
    }
    public TimeSpan Duration => EndTime - StartTime;
}
