using Domain.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common.ValueObjects.Abstractions;

public abstract record GuidValueObject
{
    public Guid Value { get; }

    protected GuidValueObject(Guid value, Error error)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException(error);
        }

        Value = value;
    }

    public static implicit operator Guid(GuidValueObject guidObject) => guidObject.Value;
    public override string ToString() => Value.ToString();
}
