using Domain.Common;
using Domain.Common.Exceptions;
using Domain.Common.ValueObjects.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Sessions.ValueObjects;

public record Title : StringValueObject
{
    private const int Min = 3;
    private const int Max = 20;
    public Title(string value) : base(value, Min, Max) { }

    public static Result<Title> Create(string value) =>
        IsInvalid(value, Min, Max)
            ? Result.Failure<Title>(DomainErrors.Validation.InvalidFormat)
            : new Title(value);
}
public record Instructor : StringValueObject
{
    private const int Min = 3;
    private const int Max = 100;
    public Instructor(string value) : base(value, Min, Max) { }

    public static Result<Instructor> Create(string value) =>
        IsInvalid(value, Min, Max) 
            ? Result.Failure<Instructor>(DomainErrors.Validation.InvalidFormat) 
            : new Instructor(value);
}
public record Description : StringValueObject
{
    private const int Min = 5;
    private const int Max = 500;
    public Description(string value) : base(value, Min, Max) { }

    public static Result<Description> Create(string value) =>
        IsInvalid(value, Min, Max)
            ? Result.Failure<Description>(DomainErrors.Validation.InvalidFormat)
            : new Description(value);
}
public record Capacity : RangeValueObject
{
    private const int Min = 10;
    private const int Max = 40;

    public Capacity(int value)
        : base(value, Min, Max, DomainErrors.Session.InvalidCapacity)
    {
    }

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

    public static Result<TimeSlot> Create(DateTime start, DateTime end)
    {
        if (end <= start || start < DateTime.UtcNow)
            return Result.Failure<TimeSlot>(DomainErrors.Session.InvalidDate);

        return new TimeSlot(start, end);
    }
}
