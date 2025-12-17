namespace TurnosApp.Contracts.Requests;

public record UpdateProfessionalRequest
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string LicenseNumber { get; init; } = string.Empty;
    public int SpecialtyId { get; init; }
    public bool IsActive { get; init; } = true;
}
