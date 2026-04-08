using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using Domain.Users.Enums;

namespace Domain.Users.Entities;

public sealed class UserEntity : BaseEntity<UserId>, IAggregateRoot
{
    public MembershipEntity? Membership { get; private set; }
    public FirstName? FirstName { get; private set; }
    public LastName? LastName { get; private set; }
    public Email Email { get; private set; } = null!;
    public PhoneNumber? Phone { get; private set; }
    public UserRole Role { get; private set; }
    public string? ProfileImageUrl { get; private set; }

    public bool HasActiveMembership => Membership is { IsEligibleToBook: true };

    private UserEntity() { }

    private UserEntity(UserId id, Email email, UserRole role)
    {
        Id = id;
        Email = email;
        Role = role;
    }

    public static UserEntity Register(UserId id, Email email)
    {
        return new UserEntity(id, email, UserRole.Member);
    }

    public Result StartMembership(MembershipType type)
    {
        if (Membership != null)
        {
            return Membership.ChangeType(type);
        }

        var membershipResult = MembershipEntity.CreateInternal(this.Id, type);

        if (membershipResult.IsFailure)
            return Result.Failure(membershipResult.Error);

        Membership = membershipResult.Value;
        Modified = DateTime.UtcNow;

        return Result.Success();
    }

    public Result CancelMembership()
    {
        if (Membership == null)
            return Result.Failure(DomainErrors.User.Ineligible);

        Membership = null;
        Modified = DateTime.UtcNow;

        return Result.Success();
    }

    public Result CompleteProfile(FirstName firstName, LastName lastName, PhoneNumber? phone)
    {
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        Modified = DateTime.UtcNow;
        return Result.Success();
    }

    public void UpdateProfile(
        FirstName firstName,
        LastName lastName,
        Email email,
        PhoneNumber? phone,
        string? profileImageUrl)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        ProfileImageUrl = profileImageUrl;
        Modified = DateTime.UtcNow;
    }

    public Result CanBePermanentlyDeleted()
    {
        if (Role == UserRole.Admin)
        {
            return Result.Failure(DomainErrors.User.Ineligible);
        }

        return Result.Success();
    }
}
