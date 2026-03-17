using Domain.Common.ValueObjects.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common.ValueObjects.Shared;

public record MemberId : GuidValueObject
{
    private MemberId(Guid value) : base(value, DomainErrors.Membership.NotFound) { }

    public static Result<MemberId> Create(Guid value)
    {
        if (value == Guid.Empty)
            return Result.Failure<MemberId>(DomainErrors.Membership.NotFound);

        return Result.Success(new MemberId(value));
    }

    public static MemberId New() => new(Guid.NewGuid());

    public static implicit operator Guid(MemberId id) => id.Value;
}