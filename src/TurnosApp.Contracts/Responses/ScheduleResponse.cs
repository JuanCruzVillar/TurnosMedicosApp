using System;

namespace TurnosApp.Contracts.Responses;

public record ScheduleResponse
{
    public int Id { get; init; }
    public int ProfessionalId { get; init; }
    public DayOfWeek DayOfWeek { get; init; }
    public string StartTime { get; init; } = string.Empty; // HH:mm format
    public string EndTime { get; init; } = string.Empty; // HH:mm format
    public bool IsActive { get; init; }
}

