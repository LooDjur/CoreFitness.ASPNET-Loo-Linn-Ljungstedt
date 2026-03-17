using Domain.Common;
using Domain.Common.Exceptions;
using Domain.Common.ValueObjects.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Domain.Memberships.ValueObjects;

public record FirstName : StringValueObject
{
    private const int Min = 2;
    private const int Max = 50;

    private FirstName(string value) : base(value, Min, Max) { }

    public static Result<FirstName> Create(string value) =>
        IsInvalid(value, Min, Max)
            ? Result.Failure<FirstName>(DomainErrors.Validation.InvalidFormat)
            : Result.Success(new FirstName(value));
}
public record LastName : StringValueObject
{
    private const int Min = 2;
    private const int Max = 50;

    private LastName(string value) : base(value, Min, Max) { }

    public static Result<LastName> Create(string value) =>
        IsInvalid(value, Min, Max)
            ? Result.Failure<LastName>(DomainErrors.Validation.InvalidFormat)
            : Result.Success(new LastName(value));
}
public record Email : StringValueObject
{
    private const int Min = 5;
    private const int Max = 100;

    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private Email(string value) : base(value, Min, Max) { }

    public static Result<Email> Create(string value)
    {
        if (IsInvalid(value, Min, Max) || !EmailRegex.IsMatch(value))
        {
            return Result.Failure<Email>(DomainErrors.Validation.InvalidFormat);
        }

        return Result.Success(new Email(value));
    }
}