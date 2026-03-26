using Domain.Common.ValueObjects.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common.ValueObjects.Shared;

public record FirstName : StringValueObject
{
    private const int Min = 2;
    private const int Max = 50;

    private FirstName() : base() { }
    private FirstName(string value) : base(value) { }

    public static Result<FirstName> Create(string value) =>
        IsInvalid(value, Min, Max)
            ? Result.Failure<FirstName>(DomainErrors.Validation.InvalidFormat)
            : new FirstName(value.Trim());
}

public record LastName : StringValueObject
{
    private const int Min = 2;
    private const int Max = 100;

    private LastName() : base() { }
    private LastName(string value) : base(value) { }

    public static Result<LastName> Create(string value) =>
        IsInvalid(value, Min, Max)
            ? Result.Failure<LastName>(DomainErrors.Validation.InvalidFormat)
            : new LastName(value.Trim());
}

public record Email : EmailValueObject
{
    private Email() : base() { }
    private Email(string value) : base(value) { }

    public static Result<Email> Create(string value)
    {
        if (IsInvalidFormat(value))
        {
            return Result.Failure<Email>(DomainErrors.Validation.InvalidFormat);
        }

        var normalized = value.Trim().ToLowerInvariant();

        return new Email(normalized);
    }
}

public record PhoneNumber : StringValueObject
{
    private const int Max = 20;

    private PhoneNumber() : base() { }
    private PhoneNumber(string value) : base(value) { }

    public static Result<PhoneNumber> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Success<PhoneNumber>(null!);

        return value.Length > Max
            ? Result.Failure<PhoneNumber>(DomainErrors.Validation.InvalidFormat)
            : new PhoneNumber(value);
    }
}
