using TurnosApp.Domain.Common;

namespace TurnosApp.Domain.Entities;

public class Patient : BaseEntity
{
    public int UserId { get; set; }
    public string? DocumentNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string? MedicalInsurance { get; set; }
    public string? InsuranceNumber { get; set; }
    
    public User User { get; set; } = null!;
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
