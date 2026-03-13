using Domain.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Sessions.ValueObjects;

public record SessionDescription
{
    public string Value { get; }

    public SessionDescription(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Description cannot be empty.");

        if (value.Length < 50)
            throw new DomainException("Description is too short. Please provide more details.");

        if (value.Length > 400)
            throw new DomainException("Description cannot exceed 1000 characters.");

        Value = value;
    }

    public static implicit operator string(SessionDescription description) => description.Value;
}
