using Domain.Common;
using Domain.Common.ValueObjects.Shared;
using Domain.Memberships.Enums;
using Domain.Memberships.ValueObjects;

namespace Domain.Memberships.Entities;

public sealed class MembershipEntity : BaseEntity, IAggregateRoot
{
    public FirstName FirstName { get; private set; } = null!;
    public LastName LastName { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public string? ProfileImageUrl { get; private set; }

    public MembershipStatus Status { get; private set; }
    public MembershipType Type { get; private set; }

    public DateTime ExpiryDate { get; private set; }
    public bool IsEligibleToBook => Status == MembershipStatus.Active && ExpiryDate > DateTime.UtcNow;

    private MembershipEntity() { }

    private MembershipEntity(Guid memberId, FirstName firstName, LastName lastName, Email email)
    {
        Id = memberId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Status = MembershipStatus.Active;
        Type = MembershipType.Standard;
        ExpiryDate = DateTime.UtcNow.AddYears(1);
    }

    public static Result<MembershipEntity> Create(MemberId memberId, FirstName firstName, LastName lastName, Email email)
    {
        var membership = new MembershipEntity(memberId, firstName, lastName, email);
        return membership;
    }

    public Result UpdateProfile(FirstName firstName, LastName lastName, Email email, string? imageUrl)
    {
        if (Status == MembershipStatus.Suspended)
        {
            return Result.Failure(DomainErrors.Membership.Ineligible);
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
            return Result.Failure(DomainErrors.Validation.InvalidFormat);
        }

        if (months > 2)
        {
            return Result.Failure(DomainErrors.Membership.LimitReached);
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
            return Result.Failure(DomainErrors.Membership.Ineligible);
        }

        Status = newStatus;
        Modified = DateTime.UtcNow;

        return Result.Success();
    }

    public Result AdminChangeType(MembershipType newType)
    {
        if (Type == newType)
        {
            return Result.Failure(DomainErrors.Membership.Ineligible);
        }

        Type = newType;
        Modified = DateTime.UtcNow;

        return Result.Success();
    }
}