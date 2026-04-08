using Domain.Common;
using Domain.Common.ValueObjects.Shared;
using Domain.Users.Enums;

namespace Domain.Users.Entities;

public sealed class MembershipEntity : BaseEntity<MemberId>
{
    public UserId UserId { get; private set; } = null!;
    public MembershipStatus Status { get; private set; }
    public MembershipType Type { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public bool IsEligibleToBook => Status == MembershipStatus.Active && ExpiryDate > DateTime.UtcNow;

    private MembershipEntity() { }

    private MembershipEntity(MemberId memberId, UserId userId, MembershipType type)
    {
        Id = memberId;
        UserId = userId;
        Status = MembershipStatus.Active;
        Type = type;
        ExpiryDate = DateTime.UtcNow.AddYears(1);
    }

    internal static Result<MembershipEntity> CreateInternal(UserId userId, MembershipType type)
    {
        if (userId == null || userId.Value == Guid.Empty)
            return Result.Failure<MembershipEntity>(DomainErrors.Validation.Required);

        var membership = new MembershipEntity(MemberId.New(), userId, type);

        return Result.Success(membership);
    }

    public Result ChangeType(MembershipType newType)
    {
        if (Type == newType) return Result.Failure(DomainErrors.User.Ineligible);

        Type = newType;
        Modified = DateTime.UtcNow;
        return Result.Success();
    }

    public Result AdminExtend(int months)
    {
        if (months <= 0) return Result.Failure(DomainErrors.Validation.InvalidFormat);
        if (months > 2) return Result.Failure(DomainErrors.User.LimitReached);

        ExpiryDate = ExpiryDate < DateTime.UtcNow
            ? DateTime.UtcNow.AddMonths(months)
            : ExpiryDate.AddMonths(months);

        Modified = DateTime.UtcNow;
        return Result.Success();
    }

    public Result AdminUpdateStatus(MembershipStatus newStatus)
    {
        if (Status == newStatus) return Result.Failure(DomainErrors.User.Ineligible);

        Status = newStatus;
        Modified = DateTime.UtcNow;
        return Result.Success();
    }
}