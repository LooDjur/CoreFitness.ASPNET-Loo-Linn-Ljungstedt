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

    public static SessionEntity Create(SessionTitle title, SessionInstructor instructor, SessionCategory category, SessionTimeSlot schedule, SessionCapacity maxCapacity, SessionDescription description)
    {
        return new SessionEntity(title, instructor, category, schedule, maxCapacity, description);
    }

    public void UpdateDetails(SessionTitle title, SessionInstructor instructor, SessionDescription description, SessionCategory category, SessionTimeSlot schedule, SessionCapacity maxCapacity)
    {
        Title = title;
        Instructor = instructor;
        Description = description;
        Category = category;
        Schedule = schedule;
        MaxCapacity = maxCapacity;

        Modified = DateTime.UtcNow;
    }
    public void Delete()
    {
        IsDeleted = true;
        Modified = DateTime.UtcNow;
    }
}
