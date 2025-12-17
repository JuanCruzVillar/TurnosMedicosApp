namespace TurnosApp.Contracts.Responses;

public record AvailableSlotResponse
{
    public DateTime DateTime { get; init; }
    public int DurationMinutes { get; init; }
    public bool IsAvailable { get; init; }
}
