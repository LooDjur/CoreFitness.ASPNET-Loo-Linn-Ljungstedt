using Domain.Common.ValueObjects.Primitives;

namespace Domain.Common.ValueObjects.Shared;

public record UserId : GuidValueObject
{
    private UserId() : base() { }
    private UserId(Guid value) : base(value) { }

    public static Result<UserId> Create(Guid value)
    {
        if (value == Guid.Empty)
            return Result.Failure<UserId>(DomainErrors.Validation.Required);

        return Result.Success(new UserId(value));
    }

    public static implicit operator Guid(UserId id) => id.Value;
}

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
        if (string.IsNullOrWhiteSpace(value) || !value.Contains('@') || IsInvalidFormat(value))
        {
            return Result.Failure<Email>(DomainErrors.Validation.InvalidFormat);
        }

        var normalized = value.Trim().ToLowerInvariant();

        return Result.Success(new Email(normalized));
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
        {
            return Result.Success(new PhoneNumber(string.Empty));
        }

        if (value.Length > Max)
        {
            return Result.Failure<PhoneNumber>(DomainErrors.Validation.InvalidFormat);
        }

        return Result.Success(new PhoneNumber(value.Trim()));
    }
}
