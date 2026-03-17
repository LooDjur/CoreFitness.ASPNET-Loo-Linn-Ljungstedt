using Domain.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common.ValueObjects.Abstractions;

public abstract record RangeValueObject
{
    public int Value { get; }

    protected RangeValueObject(int value, int min, int max, Error error)
    {
        if (IsInvalid(value, min, max))
        {
            throw new DomainException(error);
        }

        Value = value;
    }
    protected static bool IsInvalid(int value, int min, int max) => value < min || value > max;

    public static implicit operator int(RangeValueObject rangeObject) => rangeObject.Value;
}
