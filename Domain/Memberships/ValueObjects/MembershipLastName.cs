using Domain.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Memberships.ValueObjects;

public record MembershipLastName
{
    public string Value { get; }

    public MembershipLastName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Last name cannot be empty.");

        if (value.Length > 30)
            throw new DomainException("Last name is too long.");

        Value = value;
    }

    public static implicit operator string(MembershipLastName name) => name.Value;
}
