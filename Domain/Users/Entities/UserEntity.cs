using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using Domain.Users.Enums;
using System;
using System.Collections.Generic;
using System.Text;

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

    private UserEntity() { }

    private UserEntity(UserId id, Email email, UserRole role, FirstName? firstName = null, LastName? lastName = null)
    {
        Id = id;
        Email = email;
        Role = role;
        FirstName = firstName;
        LastName = lastName;
    }
    public void SetMembership(MembershipEntity membership)
    {
        Membership = membership;
    }
    public static UserEntity Register(Email email)
    {
        return new UserEntity(UserId.New(), email, UserRole.Member);
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
}