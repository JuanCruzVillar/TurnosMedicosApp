using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TurnosApp.Contracts.Responses;
using TurnosApp.Domain.Enums;
using TurnosApp.Infrastructure.Data;

namespace TurnosApp.API.Controllers.Admin;

[ApiController]
[Route("api/Admin/[controller]")]
[Authorize(Roles = "Admin")]
public class DashboardController : ControllerBase
{
    private readonly TurnosDbContext _context;

    public DashboardController(TurnosDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtiene estadísticas generales del sistema
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<DashboardStatsResponse>> GetStats()
    {
        var today = DateTime.Today;
        var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        var last7Days = today.AddDays(-7);

        // Contadores básicos
        var totalPatients = await _context.Patients.CountAsync();
        var totalProfessionals = await _context.Professionals
            .Include(p => p.User)
            .CountAsync(p => p.User.IsActive);
        var totalSpecialties = await _context.Specialties.CountAsync();
        var totalAppointments = await _context.Appointments.CountAsync();

        // Turnos por período
        var appointmentsToday = await _context.Appointments
            .CountAsync(a => a.DateTime.Date == today);

        var appointmentsThisWeek = await _context.Appointments
            .CountAsync(a => a.DateTime.Date >= startOfWeek && a.DateTime.Date <= today);

        var appointmentsThisMonth = await _context.Appointments
            .CountAsync(a => a.DateTime.Date >= startOfMonth);

        // Turnos por estado
        var pendingAppointments = await _context.Appointments
            .CountAsync(a => a.Status == AppointmentStatus.Scheduled || 
                           a.Status == AppointmentStatus.Confirmed);

        var completedAppointments = await _context.Appointments
            .CountAsync(a => a.Status == AppointmentStatus.Completed);

        var canceledAppointments = await _context.Appointments
            .CountAsync(a => a.Status == AppointmentStatus.Canceled);

        // Estadísticas por especialidad
        var specialtiesStats = await _context.Specialties
            .Select(s => new SpecialtyStatsResponse
            {
                SpecialtyId = s.Id,
                SpecialtyName = s.Name,
                AppointmentCount = s.Professionals
                    .SelectMany(p => p.Appointments)
                    .Count(),
                ProfessionalCount = s.Professionals.Count(p => p.User.IsActive)
            })
            .OrderByDescending(s => s.AppointmentCount)
            .ToListAsync();

        // Top 5 profesionales con más turnos
        var topProfessionals = await _context.Professionals
            .Include(p => p.User)
            .Include(p => p.Specialty)
            .Include(p => p.Appointments)
            .Where(p => p.User.IsActive)
            .Select(p => new TopProfessionalResponse
            {
                ProfessionalId = p.Id,
                ProfessionalName = $"{p.User.FirstName} {p.User.LastName}",
                SpecialtyName = p.Specialty.Name,
                AppointmentCount = p.Appointments
                    .Count(a => a.Status != AppointmentStatus.Canceled)
            })
            .OrderByDescending(p => p.AppointmentCount)
            .Take(5)
            .ToListAsync();

        // Turnos por día (últimos 7 días)
        var appointmentsByDay = await _context.Appointments
            .Where(a => a.DateTime.Date >= last7Days)
            .GroupBy(a => a.DateTime.Date)
            .Select(g => new AppointmentsByDayResponse
            {
                Date = g.Key,
                Count = g.Count()
            })
            .OrderBy(a => a.Date)
            .ToListAsync();

        var response = new DashboardStatsResponse
        {
            TotalPatients = totalPatients,
            TotalProfessionals = totalProfessionals,
            TotalSpecialties = totalSpecialties,
            TotalAppointments = totalAppointments,
            AppointmentsToday = appointmentsToday,
            AppointmentsThisWeek = appointmentsThisWeek,
            AppointmentsThisMonth = appointmentsThisMonth,
            PendingAppointments = pendingAppointments,
            CompletedAppointments = completedAppointments,
            CanceledAppointments = canceledAppointments,
            SpecialtiesStats = specialtiesStats,
            TopProfessionals = topProfessionals,
            AppointmentsByDay = appointmentsByDay
        };

        return Ok(response);
    }

    /// <summary>
    /// Obtiene todos los usuarios del sistema
    /// </summary>
    [HttpGet("users")]
    public async Task<ActionResult> GetAllUsers()
    {
        var users = await _context.Users
            .Where(u => u.IsActive)
            .Select(u => new
            {
                u.Id,
                u.Email,
                u.FirstName,
                u.LastName,
                u.PhoneNumber,
                Role = u.Role.ToString(),
                u.IsActive,
                u.CreatedAt
            })
            .OrderBy(u => u.Role)
            .ThenBy(u => u.LastName)
            .ToListAsync();

        return Ok(users);
    }

    /// <summary>
    /// Obtiene turnos recientes
    /// </summary>
    [HttpGet("recent-appointments")]
    public async Task<ActionResult> GetRecentAppointments([FromQuery] int take = 10)
    {
        var appointments = await _context.Appointments
            .Include(a => a.Professional).ThenInclude(p => p.User)
            .Include(a => a.Professional).ThenInclude(p => p.Specialty)
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .OrderByDescending(a => a.CreatedAt)
            .Take(take)
            .Select(a => new
            {
                a.Id,
                a.DateTime,
                Status = a.Status.ToString(),
                Professional = new
                {
                    Name = $"{a.Professional.User.FirstName} {a.Professional.User.LastName}",
                    Specialty = a.Professional.Specialty.Name
                },
                Patient = new
                {
                    Name = $"{a.Patient.User.FirstName} {a.Patient.User.LastName}",
                    a.Patient.User.Email
                },
                a.Reason,
                a.CreatedAt
            })
            .ToListAsync();

        return Ok(appointments);
    }
}
