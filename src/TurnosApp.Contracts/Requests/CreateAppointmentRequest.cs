using System.ComponentModel.DataAnnotations;

namespace TurnosApp.Contracts.Requests;

public record CreateAppointmentRequest
{
    [Required(ErrorMessage = "El ID del profesional es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID del profesional debe ser mayor a 0")]
    public int ProfessionalId { get; init; }

    [Required(ErrorMessage = "La fecha y hora del turno es requerida")]
    public DateTime DateTime { get; init; }

    [MaxLength(500, ErrorMessage = "El motivo no puede exceder 500 caracteres")]
    public string? Reason { get; init; }
}
