using Domain.Common;
using Domain.Common.Exceptions;
using Domain.Common.ValueObjects.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Domain.Memberships.ValueObjects;
public record FirstName(string Value) : StringValueObject(Value, 2, 50);
public record LastName(string Value) : StringValueObject(Value, 2, 50);
public record Email : StringValueObject
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public Email(string value) : base(value, 5, 100)
    {
        if (!EmailRegex.IsMatch(value))
        {
            throw new DomainException(DomainErrors.Validation.InvalidFormat);
        }
    }
}