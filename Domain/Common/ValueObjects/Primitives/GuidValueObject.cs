using Domain.Bookings.ValueObjects;
using Domain.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common.ValueObjects.Primitives;

public abstract record GuidValueObject
{
    public Guid Value { get; private init; }
    protected GuidValueObject() { }

    protected GuidValueObject(Guid value) => Value = value;

    protected static bool IsInvalid(Guid value) => value == Guid.Empty;

    public static implicit operator Guid(GuidValueObject guidObject) => guidObject.Value;
    public override string ToString() => Value.ToString();
}
