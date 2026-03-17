using Domain.Common;
using Domain.Common.ValueObjects.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Bookings.ValueObjects;

public record SessionId : GuidValueObject
{
    private SessionId(Guid value) : base(value, DomainErrors.Session.NotFound) { }

    public static Result<SessionId> Create(Guid value)
    {
        if (value == Guid.Empty)
            return Result.Failure<SessionId>(DomainErrors.Session.NotFound);

        return Result.Success(new SessionId(value));
    }

    public static SessionId New() => new(Guid.NewGuid());

    public static implicit operator Guid(SessionId id) => id.Value;
}