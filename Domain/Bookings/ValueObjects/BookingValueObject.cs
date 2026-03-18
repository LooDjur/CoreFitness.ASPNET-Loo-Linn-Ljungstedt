using Domain.Common;
using Domain.Common.ValueObjects.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Bookings.ValueObjects;

public record BookingId : GuidValueObject
{
    private BookingId() : base() { }
    private BookingId(Guid value) : base(value) { }

    public static BookingId New() => new(Guid.NewGuid());
    public static Result<BookingId> Create(Guid value)
    {
        if (value == Guid.Empty) return Result.Failure<BookingId>(DomainErrors.Validation.InvalidFormat);
        return Result.Success(new BookingId(value));
    }

    public static implicit operator Guid(BookingId id) => id.Value;
}