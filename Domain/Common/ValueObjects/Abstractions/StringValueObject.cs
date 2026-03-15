using Domain.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common.ValueObjects.Abstractions;

public abstract record StringValueObject
{
    public string Value { get; }

    protected StringValueObject(string value, int minLength, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(DomainErrors.Validation.Required);
        }

        if (value.Length < minLength || value.Length > maxLength)
        {
            throw new DomainException(DomainErrors.Validation.InvalidFormat);
        }

        Value = value;
    }

    public static implicit operator string(StringValueObject valueObject) => valueObject.Value;

    public override string ToString() => Value;
}
