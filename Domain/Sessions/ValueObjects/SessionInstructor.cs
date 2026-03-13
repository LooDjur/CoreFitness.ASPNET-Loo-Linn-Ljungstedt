using Domain.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Sessions.ValueObjects;

public record SessionInstructor
{
    public string Value { get; }
    public SessionInstructor(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException("Instructor name cannot be empty.");
        Value = value;
    }
    public static implicit operator string(SessionInstructor name) => name.Value;
}
