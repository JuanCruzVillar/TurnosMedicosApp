namespace TurnosApp.Contracts.Responses;

public record ProfessionalResponse
{
    public int Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string LicenseNumber { get; init; } = string.Empty;
    public SpecialtyResponse Specialty { get; init; } = null!;
}
