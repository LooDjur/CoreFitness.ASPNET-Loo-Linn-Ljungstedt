using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using Domain.ContactReq.ValueObjects;

namespace Domain.ContactReq.Entities;

public sealed class ContactRequestEntity : BaseEntity<ContactRequestId>, IAggregateRoot
{
    public FirstName FirstName { get; private set; } = null!;
    public LastName LastName { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public PhoneNumber? Phone { get; private set; }
    public MessageBody Message { get; private set; } = null!;

    private ContactRequestEntity() { }

    private ContactRequestEntity(
        ContactRequestId id,
        FirstName firstName,
        LastName lastName,
        Email email,
        PhoneNumber? phone,
        MessageBody message)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        Message = message;
    }

    public static Result<ContactRequestEntity> Create(
        FirstName firstName,
        LastName lastName,
        Email email,
        PhoneNumber? phone,
        MessageBody message)
    {
        var id = ContactRequestId.New();

        return Result.Success(new ContactRequestEntity(
            id,
            firstName,
            lastName,
            email,
            phone,
            message));
    }
}