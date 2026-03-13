using Domain.Common;
using Domain.Sessions.Enums;
using Domain.Sessions.ValueObjects;

namespace Domain.Sessions.Entities;

public sealed class SessionEntity : BaseEntity, IAggregateRoot
{
    public SessionTitle Title { get; private set; } = null!;
    public SessionDescription Description { get; private set; } = null!;
    public SessionInstructor Instructor { get; private set; } = null!;
    public SessionCategory Category { get; private set; }
    public SessionTimeSlot Schedule { get; private set; } = null!;
    public SessionCapacity MaxCapacity { get; private set; } = null!;
    public bool IsDeleted { get; private set; } = false;

    private SessionEntity() { }

    private SessionEntity(SessionTitle title, SessionInstructor instructor, SessionCategory category, SessionTimeSlot schedule, SessionCapacity maxCapacity, SessionDescription description)
    {
        Title = title;
        Instructor = instructor;
        Category = category;
        Schedule = schedule;
        MaxCapacity = maxCapacity;
        Description = description;
    }

    public static Result<SessionEntity> Create(SessionTitle title, SessionInstructor instructor, SessionCategory category, SessionTimeSlot schedule, SessionCapacity maxCapacity, SessionDescription description)
    {
        if (schedule.StartTime < DateTime.UtcNow)
        {
            return Result.Failure<SessionEntity>("You can only create upcoming sessions.");
        }

        var session = new SessionEntity(title, instructor, category, schedule, maxCapacity, description);

        return session;
    }

    public Result UpdateDetails(
        SessionTitle title,
        SessionInstructor instructor,
        SessionDescription description,
        SessionCategory category,
        SessionTimeSlot schedule,
        SessionCapacity maxCapacity)
    {
        if (Schedule.StartTime < DateTime.UtcNow)
        {
            return Result.Failure("Cannot update a session that has already taken place.");
        }

        if (schedule.StartTime < DateTime.UtcNow)
        {
            return Result.Failure("The new session time cannot be in the past.");
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
        if (Schedule.StartTime < DateTime.UtcNow && !IsDeleted)
        {
            return Result.Failure("Cannot delete a completed session from history.");
        }

        if (IsDeleted)
        {
            return Result.Failure("Session is already deleted.");
        }

        IsDeleted = true;
        Modified = DateTime.UtcNow;

        return Result.Success();
    }
}
