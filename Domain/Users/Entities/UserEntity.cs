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

    public bool HasActiveMembership(DateTime utcNow) =>
        Membership is not null && Membership.IsEligibleToBook(utcNow);

    private UserEntity() { }

    private UserEntity(UserId id, Email email, UserRole role, DateTime utcNow)
    {
        Initialize(id, utcNow);

        Email = email;
        Role = role;
    }

    public static UserEntity Register(UserId id, Email email, DateTime utcNow, UserRole role = UserRole.Member)
    {
        return new UserEntity(id, email, role, utcNow);
    }

    public Result StartMembership(MembershipType type, DateTime utcNow)
    {
        if (Membership != null)
        {
            if (Membership.IsDeleted)
            {
                Membership.Reactivate(type, utcNow);
                UpdateModified(utcNow);
                return Result.Success();
            }

            return Membership.ChangeType(type, utcNow);
        }

        var membershipResult = MembershipEntity.CreateInternal(this.Id, type, utcNow);

        if (membershipResult.IsFailure)
            return Result.Failure(membershipResult.Error);

        Membership = membershipResult.Value;
        UpdateModified(utcNow);

        return Result.Success();
    }

    public Result CancelMembership(DateTime utcNow)
    {
        if (Membership == null)
            return Result.Failure(DomainErrors.User.Ineligible);

        Membership.Cancel(utcNow);
        UpdateModified(utcNow);

        return Result.Success();
    }

    public Result CompleteProfile(FirstName firstName, LastName lastName, PhoneNumber? phone, DateTime utcNow)
    {
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;

        UpdateModified(utcNow);
        return Result.Success();
    }

    public void UpdateProfile(
        FirstName firstName,
        LastName lastName,
        Email email,
        PhoneNumber? phone,
        string? profileImageUrl,
        DateTime utcNow)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        ProfileImageUrl = profileImageUrl;

        UpdateModified(utcNow);
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