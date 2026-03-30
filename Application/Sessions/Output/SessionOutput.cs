namespace Application.Sessions.Output;

public record SessionOutput(
    Guid Id,
    string Title,
    string Description,
    string Instructor,
    string Category,
    DateTime StartTime,
    DateTime EndTime,
    int MaxCapacity
);