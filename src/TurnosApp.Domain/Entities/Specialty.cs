using TurnosApp.Domain.Common;

namespace TurnosApp.Domain.Entities;

public class Specialty : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DurationMinutes { get; set; } = 30;
    
    public ICollection<Professional> Professionals { get; set; } = new List<Professional>();
}
