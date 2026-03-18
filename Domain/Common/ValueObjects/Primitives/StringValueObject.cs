using Domain.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common.ValueObjects.Primitives;

public abstract record StringValueObject
{
    public string Value { get; private init; } = null!;

    protected StringValueObject() { }
    protected StringValueObject(string value)
    {
        Value = value;
    }

    protected static bool IsInvalid(string value, int min, int max)
    {
        if (string.IsNullOrWhiteSpace(value)) return true;
        return value.Length < min || value.Length > max;
    }

    public static implicit operator string(StringValueObject valueObject) => valueObject.Value;

    public override string ToString() => Value;
}