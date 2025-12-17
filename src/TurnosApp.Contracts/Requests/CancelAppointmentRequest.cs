namespace TurnosApp.Contracts.Requests;

public record CancelAppointmentRequest
{
    public string Reason { get; init; } = string.Empty;
}
