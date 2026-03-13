using Domain.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Bookings.ValueObjects;

public record BookingMemberId
{
    public string Value { get; }
    public BookingMemberId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Member ID cannot be empty.");
        Value = value;
    }
    public static implicit operator string(BookingMemberId id) => id.Value;
}