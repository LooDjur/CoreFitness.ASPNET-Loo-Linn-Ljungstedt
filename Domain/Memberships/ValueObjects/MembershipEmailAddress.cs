using Domain.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Memberships.ValueObjects;

public record MembershipEmailAddress
{
    public string Value { get; }

    public MembershipEmailAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.Contains('@'))
            throw new DomainException("Invalid email format.");

        Value = value;
    }

    public static implicit operator string(MembershipEmailAddress email) => email.Value;
}