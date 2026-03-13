using Domain.Common;
using Domain.Sessions.Enums;
using Domain.Sessions.ValueObjects;

namespace Domain.Sessions.Entities;

public sealed class SessionEntity : BaseEntity, IAggregateRoot
{
    public Title Title { get; private set; } = null!;
    public Description Description { get; private set; } = null!;
    public Instructor Instructor { get; private set; } = null!;
    public SessionCategory Category { get; private set; }
    public TimeSlot Schedule { get; private set; } = null!;
    public Capacity MaxCapacity { get; private set; } = null!;
    public bool IsDeleted { get; private set; } = false;

    private SessionEntity() { }

    private SessionEntity(Title title, Instructor instructor, SessionCategory category, TimeSlot schedule, Capacity maxCapacity, Description description)
    {
        Title = title;
        Instructor = instructor;
        Category = category;
        Schedule = schedule;
        MaxCapacity = maxCapacity;
        Description = description;
    }

    public static Result<SessionEntity> Create(Title title, Instructor instructor, SessionCategory category, TimeSlot schedule, Capacity maxCapacity, Description description)
    {
        if (schedule.StartTime < DateTime.UtcNow)
        {
            return Result.Failure<SessionEntity>(DomainErrors.Session.InvalidDate);
        }

        var session = new SessionEntity(title, instructor, category, schedule, maxCapacity, description);

        return session;
    }

    public Result UpdateDetails(
        Title title,
        Instructor instructor,
        Description description,
        SessionCategory category,
        TimeSlot schedule,
        Capacity maxCapacity)
    {
        if (schedule.StartTime < DateTime.UtcNow)
        {
            return Result.Failure(DomainErrors.Session.InvalidDate);
        }
        Title = title;
        Instructor = instructor;
        Description = description;
        Category = category;
        Schedule = schedule;
        MaxCapacity = maxCapacity;

        Modified = DateTime.UtcNow;

        return Result.Success();
    }

    public Result Delete()
    {
        if (IsDeleted)
        {
            return Result.Failure(DomainErrors.Session.ActionNotAllowed);
        }

        if (Schedule.StartTime < DateTime.UtcNow)
        {
            return Result.Failure(DomainErrors.Session.ActionNotAllowed);
        }

        IsDeleted = true;
        Modified = DateTime.UtcNow;

        return Result.Success();
    }
}
