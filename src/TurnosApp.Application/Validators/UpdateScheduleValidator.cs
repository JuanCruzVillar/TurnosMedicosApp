using FluentValidation;
using TurnosApp.Contracts.Requests;

namespace TurnosApp.Application.Validators;

public class UpdateScheduleValidator : AbstractValidator<UpdateScheduleRequest>
{
    public UpdateScheduleValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El ID del horario es inválido");

        RuleFor(x => x.DayOfWeek)
            .IsInEnum()
            .WithMessage("Día de la semana inválido");

        RuleFor(x => x.StartTime)
            .NotEmpty()
            .WithMessage("La hora de inicio es requerida")
            .Matches(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$")
            .WithMessage("El formato de hora de inicio debe ser HH:mm (ej: 09:00)");

        RuleFor(x => x.EndTime)
            .NotEmpty()
            .WithMessage("La hora de fin es requerida")
            .Matches(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$")
            .WithMessage("El formato de hora de fin debe ser HH:mm (ej: 18:00)");

        RuleFor(x => x)
            .Must(x => IsEndTimeAfterStartTime(x.StartTime, x.EndTime))
            .WithMessage("La hora de fin debe ser posterior a la hora de inicio")
            .When(x => !string.IsNullOrEmpty(x.StartTime) && !string.IsNullOrEmpty(x.EndTime));
    }

    private bool IsEndTimeAfterStartTime(string startTime, string endTime)
    {
        if (!TimeSpan.TryParse(startTime, out var start) || 
            !TimeSpan.TryParse(endTime, out var end))
            return true;

        return end > start;
    }
}
