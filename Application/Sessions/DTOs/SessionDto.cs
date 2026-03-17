namespace Application.Sessions.DTOs;

public sealed class SessionDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string Instructor { get; init; } = null!;
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public int MaxCapacity { get; init; }
    public string Category { get; init; } = null!;
}
