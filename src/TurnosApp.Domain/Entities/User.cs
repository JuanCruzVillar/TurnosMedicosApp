using TurnosApp.Domain.Common;
using TurnosApp.Domain.Enums;

namespace TurnosApp.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    
    public Professional? Professional { get; set; }
    public Patient? Patient { get; set; }
}
