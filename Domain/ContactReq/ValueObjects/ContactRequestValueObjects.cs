using Domain.Common;
using Domain.Common.ValueObjects.Primitives;

namespace Domain.ContactReq.ValueObjects;

public record MessageBody : StringValueObject
{
    private const int Min = 2;
    private const int Max = 2000;

    private MessageBody() : base() { }
    private MessageBody(string value) : base(value) { }

    public static Result<MessageBody> Create(string value) =>
        IsInvalid(value, Min, Max)
            ? Result.Failure<MessageBody>(DomainErrors.Validation.InvalidFormat)
            : new MessageBody(value);
}

public record ContactRequestId : GuidValueObject
{
    private ContactRequestId() : base() { }
    private ContactRequestId(Guid value) : base(value) { }

    public static ContactRequestId New() => new(Guid.NewGuid());

    public static Result<ContactRequestId> Create(Guid value)
    {
        if (value == Guid.Empty)
            return Result.Failure<ContactRequestId>(DomainErrors.Validation.InvalidFormat);

        return Result.Success(new ContactRequestId(value));
    }

    public static implicit operator Guid(ContactRequestId id) => id.Value;
}