using FluentValidation;
using TurnosApp.Contracts.Requests;

namespace TurnosApp.Application.Validators;

public class CreateAppointmentValidator : AbstractValidator<CreateAppointmentRequest>
{
    public CreateAppointmentValidator()
    {
        RuleFor(x => x.ProfessionalId)
            .GreaterThan(0)
            .WithMessage("Debe seleccionar un profesional válido");

        RuleFor(x => x.DateTime)
            .GreaterThan(DateTime.Now)
            .WithMessage("La fecha y hora del turno debe ser futura");

        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .WithMessage("El motivo no puede exceder los 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Reason));
    }
}
