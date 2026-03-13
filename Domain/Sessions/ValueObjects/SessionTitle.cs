using Domain.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Sessions.ValueObjects;

public record SessionTitle
{
    public string Value { get; }
    public SessionTitle(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException("Title cannot be empty.");
        if (value.Length > 20) throw new DomainException("Title is too long.");
        Value = value;
    }
    public static implicit operator string(SessionTitle title) => title.Value;
}
