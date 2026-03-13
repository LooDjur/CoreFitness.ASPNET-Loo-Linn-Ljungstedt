using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Sessions.ValueObjects;

public record class SessionTimeSlot
{
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }

    public SessionTimeSlot(DateTime startTime, DateTime endTime)
    {
        if (endTime <= startTime)
            throw new ArgumentException("Endtime must be after starttime.");

        StartTime = startTime;
        EndTime = endTime;
    }
    public TimeSpan Duration => EndTime - StartTime;
}
