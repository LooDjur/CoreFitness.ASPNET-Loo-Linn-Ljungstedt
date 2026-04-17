using Domain.Bookings.ValueObjects;
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

    private SessionEntity(SessionId id, Title title, Description description, Instructor instructor, SessionCategory category, TimeSlot schedule, Capacity maxCapacity, DateTime utcNow)
    {
        Initialize(id, utcNow);

        Title = title;
        Description = description;
        Instructor = instructor;
        Category = category;
        Schedule = schedule;
        MaxCapacity = maxCapacity;
    }

    public static Result<SessionEntity> Create(
        Title title,
        Description description,
        Instructor instructor,
        SessionCategory category,
        TimeSlot schedule,
        Capacity maxCapacity,
        DateTime utcNow)
    {
        if (title is null || instructor is null || schedule is null || maxCapacity is null)
            return Result.Failure<SessionEntity>(DomainErrors.Validation.Required);

        var session = new SessionEntity(
            SessionId.New(),
            title,
            description,
            instructor,
            category,
            schedule,
            maxCapacity,
            utcNow);

        return Result.Success(session);
    }

    public Result UpdateDetails(
        Title title,
        Description description,
        Instructor instructor,
        SessionCategory category,
        TimeSlot schedule,
        Capacity maxCapacity,
        DateTime utcNow)
    {
        if (Schedule.StartTime < utcNow)
            return Result.Failure(DomainErrors.Session.ActionNotAllowed);

        Title = title;
        Description = description;
        Instructor = instructor;
        Category = category;
        Schedule = schedule;
        MaxCapacity = maxCapacity;

        UpdateModified(utcNow);

        return Result.Success();
    }

    public Result Delete(DateTime utcNow)
    {
        if (IsDeleted)
            return Result.Failure(DomainErrors.Session.ActionNotAllowed);

        if (Schedule.StartTime < utcNow)
            return Result.Failure(DomainErrors.Session.ActionNotAllowed);

        IsDeleted = true;
        UpdateModified(utcNow);

        return Result.Success();
    }

    public Result<BookingId> Book(int currentBookingsCount, DateTime utcNow)
    {
        if (IsDeleted)
            return Result.Failure<BookingId>(DomainErrors.Session.NotFound);

        if (Schedule.StartTime <= utcNow)
            return Result.Failure<BookingId>(DomainErrors.Session.ActionNotAllowed);

        if (currentBookingsCount >= MaxCapacity.Value)
            return Result.Failure<BookingId>(DomainErrors.Session.InvalidCapacity);

        return Result.Success(BookingId.New());
    }
}