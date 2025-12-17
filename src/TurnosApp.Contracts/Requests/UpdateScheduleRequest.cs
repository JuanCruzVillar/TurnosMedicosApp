using System;

namespace TurnosApp.Contracts.Requests;

public record UpdateScheduleRequest
{
    public int Id { get; init; }
    public DayOfWeek DayOfWeek { get; init; }
    public string StartTime { get; init; } = string.Empty; // HH:mm format
    public string EndTime { get; init; } = string.Empty; // HH:mm format
    public bool IsActive { get; init; } = true;
}

