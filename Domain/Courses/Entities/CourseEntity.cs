using Domain.Common;
using Domain.Common.Exceptions;
using Domain.Courses.Enums;
using Domain.Courses.ValueObjects;

namespace Domain.Courses.Entities;

public sealed class CourseEntity : BaseEntity, IAggregateRoot
{
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public string Instructor { get; private set; } = null!;
    public CourseCategory Category { get; private set; }
    public TimeSlot Schedule { get; private set; } = null!;
    public int MaxCapacity { get; private set; }

    private CourseEntity() { }

    public CourseEntity(string title, string instructor, CourseCategory category, TimeSlot schedule, int maxCapacity, string? description = null)
    {
        Validate(title, instructor, maxCapacity);

        Title = title;
        Instructor = instructor;
        Category = category;
        Schedule = schedule;
        MaxCapacity = maxCapacity;
        Description = description;
    }

    public void UpdateDetails(string title, string instructor, string? description, CourseCategory category, TimeSlot schedule, int maxCapacity)
    {
        Validate(title, instructor, maxCapacity);

        Title = title;
        Instructor = instructor;
        Description = description;
        Category = category;
        Schedule = schedule;
        MaxCapacity = maxCapacity;

        Modified = DateTime.UtcNow;
    }

    private static void Validate(string title, string instructor, int maxCapacity)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Title cannot be empty.");

        if (string.IsNullOrWhiteSpace(instructor))
            throw new DomainException("Session needs an insturctor..");

        if (maxCapacity <= 0)
            throw new DomainException("Minimum capacity is 10 people.");
    }
    public bool IsFull(int currentBookingCount)
    {
        return currentBookingCount >= MaxCapacity;
    }
}
