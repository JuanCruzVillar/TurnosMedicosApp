using TurnosApp.Domain.Common;
using TurnosApp.Domain.Enums;

namespace TurnosApp.Domain.Entities;

public class Appointment : BaseEntity
{
    public int ProfessionalId { get; set; }
    public int PatientId { get; set; }
    public DateTime DateTime { get; set; }
    public int DurationMinutes { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public DateTime? CanceledAt { get; set; }
    public string? CancellationReason { get; set; }
    
    public Professional Professional { get; set; } = null!;
    public Patient Patient { get; set; } = null!;
}
