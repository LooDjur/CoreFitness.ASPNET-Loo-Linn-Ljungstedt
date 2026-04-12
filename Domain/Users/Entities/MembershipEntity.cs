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

    public bool IsEligibleToBook(DateTime utcNow) =>
        Status == MembershipStatus.Active && ExpiryDate > utcNow;

    private MembershipEntity() { }

    private MembershipEntity(MemberId memberId, UserId userId, MembershipType type, DateTime utcNow)
    {
        Initialize(memberId, utcNow);

        UserId = userId;
        Status = MembershipStatus.Active;
        Type = type;
        ExpiryDate = utcNow.AddYears(1);
    }

    internal static Result<MembershipEntity> CreateInternal(UserId userId, MembershipType type, DateTime utcNow)
    {
        if (userId == null || userId.Value == Guid.Empty)
            return Result.Failure<MembershipEntity>(DomainErrors.Validation.Required);

        return Result.Success(new MembershipEntity(MemberId.New(), userId, type, utcNow));
    }

    public void Cancel(DateTime utcNow)
    {
        IsDeleted = true;
        Status = MembershipStatus.Cancelled;
        UpdateModified(utcNow);
    }
    public void Reactivate(MembershipType newType, DateTime utcNow)
    {
        IsDeleted = false;
        Status = MembershipStatus.Active;
        Type = newType;
        ExpiryDate = utcNow.AddYears(1);
        UpdateModified(utcNow);
    }
    public Result ChangeType(MembershipType newType, DateTime utcNow)
    {
        if (Type == newType) return Result.Failure(DomainErrors.User.Ineligible);

        Type = newType;
        UpdateModified(utcNow);
        return Result.Success();
    }

    public Result AdminExtend(int months, DateTime utcNow)
    {
        if (months <= 0) return Result.Failure(DomainErrors.Validation.InvalidFormat);
        if (months > 2) return Result.Failure(DomainErrors.User.LimitReached);

        ExpiryDate = ExpiryDate < utcNow
            ? utcNow.AddMonths(months)
            : ExpiryDate.AddMonths(months);

        UpdateModified(utcNow);
        return Result.Success();
    }

    public Result AdminUpdateStatus(MembershipStatus newStatus, DateTime utcNow)
    {
        if (Status == newStatus) return Result.Failure(DomainErrors.User.Ineligible);

        Status = newStatus;
        UpdateModified(utcNow);
        return Result.Success();
    }
}