using Domain.Common;
using Domain.Common.Exceptions;
using Domain.Common.ValueObjects.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Sessions.ValueObjects;

public record Title : StringValueObject
{
    private const int Min = 3;
    private const int Max = 20;

    private Title() : base() { }
    private Title(string value) : base(value) { }

    public static Result<Title> Create(string value) =>
        IsInvalid(value, Min, Max)
            ? Result.Failure<Title>(DomainErrors.Validation.InvalidFormat)
            : new Title(value);
}
public record Instructor : StringValueObject
{
    private const int Min = 3;
    private const int Max = 100;

    private Instructor() : base() { }
    private Instructor(string value) : base(value) { }

    public static Result<Instructor> Create(string value) =>
        IsInvalid(value, Min, Max) 
            ? Result.Failure<Instructor>(DomainErrors.Validation.InvalidFormat) 
            : new Instructor(value);
}
public record Description : StringValueObject
{
    private const int Min = 5;
    private const int Max = 500;

    private Description() : base() { }
    private Description(string value) : base(value) { }

    public static Result<Description> Create(string value) =>
        IsInvalid(value, Min, Max)
            ? Result.Failure<Description>(DomainErrors.Validation.InvalidFormat)
            : new Description(value);
}
public record Capacity : RangeValueObject
{
    private const int Min = 10;
    private const int Max = 40;

    private Capacity() : base() { }
    private Capacity(int value) : base(value) { }

    public static Result<Capacity> Create(int value)
    {
        if (IsInvalid(value, Min, Max))
        {
            return Result.Failure<Capacity>(DomainErrors.Session.InvalidCapacity);
        }

        return new Capacity(value);
    }
}
public record TimeSlot
{
    public DateTime StartTime { get; private init; }
    public DateTime EndTime { get; private init; }

    private TimeSlot() { }

    private TimeSlot(DateTime start, DateTime end)
    {
        if (end <= start)
            throw new DomainException(DomainErrors.Session.InvalidDate);

        if (start < DateTime.UtcNow)
            throw new DomainException(DomainErrors.Session.InvalidDate);

        StartTime = start;
        EndTime = end;
    }

    public static Result<TimeSlot> Create(DateTime start, DateTime end)
    {
        if (end <= start || start < DateTime.UtcNow)
            return Result.Failure<TimeSlot>(DomainErrors.Session.InvalidDate);

        return new TimeSlot(start, end);
    }
}
