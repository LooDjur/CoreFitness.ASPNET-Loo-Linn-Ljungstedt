using Domain.Common.ValueObjects.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common.ValueObjects.Shared;

public record MemberId : GuidValueObject
{
    private MemberId() : base() { }
    private MemberId(Guid value) : base(value) { }

    public static MemberId New() => new(Guid.NewGuid());

    public static Result<MemberId> Create(Guid value)
    {
        if (value == Guid.Empty)
            return Result.Failure<MemberId>(DomainErrors.Membership.NotFound);

        return Result.Success(new MemberId(value));
    }

    public static implicit operator Guid(MemberId id) => id.Value;
}