namespace TurnosApp.Contracts.Responses;

public record DashboardStatsResponse
{
    public int TotalPatients { get; init; }
    public int TotalProfessionals { get; init; }
    public int TotalSpecialties { get; init; }
    public int TotalAppointments { get; init; }
    public int AppointmentsToday { get; init; }
    public int AppointmentsThisWeek { get; init; }
    public int AppointmentsThisMonth { get; init; }
    public int PendingAppointments { get; init; }
    public int CompletedAppointments { get; init; }
    public int CanceledAppointments { get; init; }
    public IEnumerable<SpecialtyStatsResponse> SpecialtiesStats { get; init; } = new List<SpecialtyStatsResponse>();
    public IEnumerable<TopProfessionalResponse> TopProfessionals { get; init; } = new List<TopProfessionalResponse>();
    public IEnumerable<AppointmentsByDayResponse> AppointmentsByDay { get; init; } = new List<AppointmentsByDayResponse>();
}

public record SpecialtyStatsResponse
{
    public int SpecialtyId { get; init; }
    public string SpecialtyName { get; init; } = string.Empty;
    public int AppointmentCount { get; init; }
    public int ProfessionalCount { get; init; }
}

public record TopProfessionalResponse
{
    public int ProfessionalId { get; init; }
    public string ProfessionalName { get; init; } = string.Empty;
    public string SpecialtyName { get; init; } = string.Empty;
    public int AppointmentCount { get; init; }
}

public record AppointmentsByDayResponse
{
    public DateTime Date { get; init; }
    public int Count { get; init; }
}
