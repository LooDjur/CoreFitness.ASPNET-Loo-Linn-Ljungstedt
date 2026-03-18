using Domain.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common.ValueObjects.Primitives;

public abstract record RangeValueObject
{
    public int Value { get; private init; }

    protected RangeValueObject() { }

    protected RangeValueObject(int value)
    {
        Value = value;
    }

    protected static bool IsInvalid(int value, int min, int max) => value < min || value > max;

    public static implicit operator int(RangeValueObject rangeObject) => rangeObject.Value;
}
