namespace TurnosApp.Contracts.Requests;

public record UpdateAppointmentNotesRequest
{
    public string Notes { get; init; } = string.Empty;
}

