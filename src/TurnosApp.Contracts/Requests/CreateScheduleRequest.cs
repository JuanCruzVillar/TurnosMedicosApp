using System.ComponentModel.DataAnnotations;

namespace TurnosApp.Contracts.Requests;

public record CreateScheduleRequest
{
    [Required(ErrorMessage = "El día de la semana es requerido")]
    public DayOfWeek DayOfWeek { get; init; }

    [Required(ErrorMessage = "La hora de inicio es requerida")]
    [RegularExpression(@"^([0-1][0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "El formato de hora debe ser HH:mm (24 horas)")]
    public string StartTime { get; init; } = string.Empty;

    [Required(ErrorMessage = "La hora de fin es requerida")]
    [RegularExpression(@"^([0-1][0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "El formato de hora debe ser HH:mm (24 horas)")]
    public string EndTime { get; init; } = string.Empty;

    public bool IsActive { get; init; } = true;
}
