namespace TurnosApp.Contracts.Requests;

public record CreateProfessionalRequest
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string LicenseNumber { get; init; } = string.Empty;
    public int SpecialtyId { get; init; }
}
