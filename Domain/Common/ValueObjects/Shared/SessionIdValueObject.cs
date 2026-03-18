using Domain.Common.ValueObjects.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common.ValueObjects.Shared;

public record SessionId : GuidValueObject
{
    private SessionId() : base() { }
    private SessionId(Guid value) : base(value) { }

    public static SessionId New() => new(Guid.NewGuid());

    public static Result<SessionId> Create(Guid value)
    {
        if (value == Guid.Empty)
            return Result.Failure<SessionId>(DomainErrors.Session.NotFound);

        return Result.Success(new SessionId(value));
    }

    public static implicit operator Guid(SessionId id) => id.Value;
}