using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Domain.Common.ValueObjects.Primitives;

public abstract record EmailValueObject
{
    public string Value { get; init; } = null!;

    protected EmailValueObject() { }
    protected EmailValueObject(string value) => Value = value;

    protected static bool IsInvalidFormat(string value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    public static implicit operator string(EmailValueObject email) => email?.Value ?? string.Empty;
    public override string ToString() => Value;
}