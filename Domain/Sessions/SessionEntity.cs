using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;

namespace Domain.Sessions;

public sealed class SessionEntity : BaseEntity<SessionId>, IAggregateRoot
{
    public Title Title { get; private set; } = null!;
    public Description Description { get; private set; } = null!;
    public Instructor Instructor { get; private set; } = null!;
    public SessionCategory Category { get; private set; }
    public TimeSlot Schedule { get; private set; } = null!;
    public Capacity MaxCapacity { get; private set; } = null!;

    private SessionEntity() { }

    private SessionEntity(SessionId id, Title title, Description description, Instructor instructor, SessionCategory category, TimeSlot schedule, Capacity maxCapacity)
    {
        Id = id;
        Title = title;
        Description = description;
        Instructor = instructor;
        Category = category;
        Schedule = schedule;
        MaxCapacity = maxCapacity;
    }

    public static Result<SessionEntity> Create(Title title, Description description, Instructor instructor, SessionCategory category, TimeSlot schedule, Capacity maxCapacity)
    {
        if (title is null || instructor is null || schedule is null || maxCapacity is null)
            return Result.Failure<SessionEntity>(DomainErrors.Validation.Required);

        var newSessionId = SessionId.New();

        var session = new SessionEntity(newSessionId, title, description, instructor, category, schedule, maxCapacity);
        return Result.Success(session);
    }

    public Result UpdateDetails(
        Title title,
        Description description,
        Instructor instructor,
        SessionCategory category,
        TimeSlot schedule,
        Capacity maxCapacity)
    {
        Title = title;
        Description = description;
        Instructor = instructor;
        Category = category;
        Schedule = schedule;
        MaxCapacity = maxCapacity;

        Modified = DateTime.UtcNow;

        return Result.Success();
    }

    public Result Delete()
    {
        if (IsDeleted)
            return Result.Failure(DomainErrors.Session.ActionNotAllowed);

        var now = DateTime.UtcNow;

        if (Schedule.StartTime < now)
            return Result.Failure(DomainErrors.Session.ActionNotAllowed);

        IsDeleted = true;
        Modified = now;

        return Result.Success();
    }
}
