using FluentValidation;
using TurnosApp.Contracts.Requests;

namespace TurnosApp.Application.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
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
            .WithMessage("La contraseña debe tener al menos 6 caracteres")
            .Matches(@"[A-Z]")
            .WithMessage("La contraseña debe contener al menos una mayúscula")
            .Matches(@"[a-z]")
            .WithMessage("La contraseña debe contener al menos una minúscula")
            .Matches(@"[0-9]")
            .WithMessage("La contraseña debe contener al menos un número");

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

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20)
            .WithMessage("El teléfono no puede exceder los 20 caracteres")
            .Matches(@"^[0-9+\-\s()]*$")
            .WithMessage("El teléfono solo puede contener números y caracteres válidos")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}
