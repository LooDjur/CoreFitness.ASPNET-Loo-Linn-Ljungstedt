using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Domain.Common.ValueObjects.Primitives;

public abstract record EmailValueObject
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; init; } = null!;
    protected EmailValueObject() { }
    protected EmailValueObject(string value)
    {
        Value = value;
    }

    protected static bool IsInvalidFormat(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return true;

        return !EmailRegex.IsMatch(value);
    }

    public static implicit operator string(EmailValueObject email) => email.Value;
    public override string ToString() => Value;
}