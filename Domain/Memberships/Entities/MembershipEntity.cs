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

    public static Result<MembershipEntity> Create(string memberId, MembershipFirstName firstName, MembershipLastName lastName, MembershipEmailAddress email)
    {
        if (string.IsNullOrWhiteSpace(memberId))
        {
            return Result.Failure<MembershipEntity>("User ID must be provided from Identity.");
        }

        var membership = new MembershipEntity(memberId, firstName, lastName, email);
        return membership;
    }

    public Result UpdateProfile(MembershipFirstName firstName, MembershipLastName lastName, MembershipEmailAddress email, string? imageUrl)
    {
        if (Status == MembershipStatus.Suspended)
        {
            return Result.Failure("Cannot update profile for a suspended membership.");
        }

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        ProfileImageUrl = imageUrl;
        Modified = DateTime.UtcNow;

        return Result.Success();
    }

    public Result AdminExtendMembership(int months)
    {
        if (months <= 0)
        {
            return Result.Failure("Extension period must be at least 1 month.");
        }

        if (months > 2)
        {
            return Result.Failure("Cannot extend membership more than 2 motnhs at a time.");
        }

        ExpiryDate = ExpiryDate < DateTime.UtcNow
            ? DateTime.UtcNow.AddMonths(months)
            : ExpiryDate.AddMonths(months);

        Modified = DateTime.UtcNow;

        return Result.Success();
    }
    public Result AdminUpdateStatus(MembershipStatus newStatus)
    {
        if (Status == newStatus)
        {
            return Result.Failure($"Membership is already {newStatus}.");
        }

        Status = newStatus;
        Modified = DateTime.UtcNow;

        return Result.Success();
    }

    public Result AdminChangeType(MembershipType newType)
    {
        if (Type == newType)
        {
            return Result.Failure($"Membership is already of type {newType}.");
        }

        Type = newType;
        Modified = DateTime.UtcNow;

        return Result.Success();
    }
}