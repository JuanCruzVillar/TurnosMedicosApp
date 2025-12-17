namespace TurnosApp.Contracts.Requests;

public record CreateAppointmentRequest
{
    public int ProfessionalId { get; init; }
    public DateTime DateTime { get; init; }
    public string? Reason { get; init; }
}
