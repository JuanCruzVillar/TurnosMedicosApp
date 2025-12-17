namespace TurnosApp.Contracts.Responses;

public record AppointmentResponse
{
    public int Id { get; init; }
    public DateTime DateTime { get; init; }
    public int DurationMinutes { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? Reason { get; init; }
    public string? Notes { get; init; }
    public ProfessionalResponse Professional { get; init; } = null!;
    public PatientResponse Patient { get; init; } = null!;
}
