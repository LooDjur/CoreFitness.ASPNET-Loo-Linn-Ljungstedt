using Domain.Common;
using Domain.Common.Exceptions;
using Domain.Memberships.Enums;
using Domain.Memberships.ValueObjects;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace Domain.Memberships.Entities;

public sealed class MembershipEntity : BaseEntity, IAggregateRoot
{
    public string MemberId { get; private set; } = null!;

    public MembershipFirstName FirstName { get; private set; } = null!;
    public MembershipLastName LastName { get; private set; } = null!;
    public MembershipEmailAddress Email { get; private set; } = null!;
    public string? ProfileImageUrl { get; private set; }

    public MembershipStatus Status { get; private set; }
    public MembershipType Type { get; private set; }

    public DateTime ExpiryDate { get; private set; }
    public bool IsEligibleToBook => Status == MembershipStatus.Active && ExpiryDate > DateTime.UtcNow;

    private MembershipEntity() { }

    private MembershipEntity(string memberId, MembershipFirstName firstName, MembershipLastName lastName, MembershipEmailAddress email)
    {
        MemberId = memberId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Status = MembershipStatus.Active;
        Type = MembershipType.Standard;
        ExpiryDate = DateTime.UtcNow.AddYears(1);
    }

    public static MembershipEntity Create(string memberId, MembershipFirstName firstName, MembershipLastName lastName, MembershipEmailAddress email)
    {
        return new MembershipEntity(memberId, firstName, lastName, email);
    }

    public void AdminUpdateStatus(MembershipStatus newStatus)
    {
        Status = newStatus;
        Modified = DateTime.UtcNow;
    }

    public void AdminChangeType(MembershipType newType)
    {
        Type = newType;
        Modified = DateTime.UtcNow;
    }

    public void UpdateProfile(MembershipFirstName firstName, MembershipLastName lastName, MembershipEmailAddress email, string? imageUrl)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        ProfileImageUrl = imageUrl;
        Modified = DateTime.UtcNow;
    }

    public void AdminExtendMembership(int months)
    {
        ExpiryDate = ExpiryDate < DateTime.UtcNow
            ? DateTime.UtcNow.AddMonths(months)
            : ExpiryDate.AddMonths(months);
        Modified = DateTime.UtcNow;
    }
}