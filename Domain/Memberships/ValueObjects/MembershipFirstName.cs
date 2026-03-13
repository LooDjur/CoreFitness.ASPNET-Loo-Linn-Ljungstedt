using Domain.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Memberships.ValueObjects;

public record MembershipFirstName
{
    public string Value { get; }

    public MembershipFirstName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("First name cannot be empty.");

        if (value.Length > 20)
            throw new DomainException("First name is too long.");

        Value = value;
    }

    public static implicit operator string(MembershipFirstName name) => name.Value;
}
