using FluentValidation;
using TurnosApp.Contracts.Requests;

namespace TurnosApp.Application.Validators;

public class CreateProfessionalValidator : AbstractValidator<CreateProfessionalRequest>
{
    public CreateProfessionalValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("El email es requerido")
            .EmailAddress()
            .WithMessage("El formato del email no es válido")
            .MaximumLength(255)
            .WithMessage("El email no puede exceder los 255 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("La contraseña es requerida")
            .MinimumLength(6)
            .WithMessage("La contraseña debe tener al menos 6 caracteres");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("El nombre es requerido")
            .MaximumLength(100)
            .WithMessage("El nombre no puede exceder los 100 caracteres");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("El apellido es requerido")
            .MaximumLength(100)
            .WithMessage("El apellido no puede exceder los 100 caracteres");

        RuleFor(x => x.LicenseNumber)
            .NotEmpty()
            .WithMessage("El número de matrícula es requerido")
            .MaximumLength(50)
            .WithMessage("El número de matrícula no puede exceder los 50 caracteres")
            .Matches(@"^[A-Z]{2}-\d{6}$")
            .WithMessage("El formato de matrícula debe ser XX-NNNNNN (ej: MN-123456)");

        RuleFor(x => x.SpecialtyId)
            .GreaterThan(0)
            .WithMessage("Debe seleccionar una especialidad válida");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20)
            .WithMessage("El teléfono no puede exceder los 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}
