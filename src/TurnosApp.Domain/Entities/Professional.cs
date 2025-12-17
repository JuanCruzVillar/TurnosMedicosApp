using TurnosApp.Domain.Common;

namespace TurnosApp.Domain.Entities;

public class Professional : BaseEntity
{
    public int UserId { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public int SpecialtyId { get; set; }
    
    public User User { get; set; } = null!;
    public Specialty Specialty { get; set; } = null!;
    public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
