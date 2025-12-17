namespace TurnosApp.Contracts.Requests;

public record UpdateSpecialtyRequest
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int DurationMinutes { get; init; } = 30;
}
