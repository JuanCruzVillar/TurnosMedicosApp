using TurnosApp.Domain.Common;

namespace TurnosApp.Domain.Entities;

public class Schedule : BaseEntity
{
    public int ProfessionalId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsActive { get; set; } = true;
    
    public Professional Professional { get; set; } = null!;
}
