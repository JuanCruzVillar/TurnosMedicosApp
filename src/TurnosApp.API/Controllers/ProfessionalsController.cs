using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TurnosApp.Contracts.Responses;
using TurnosApp.Infrastructure.Data;

namespace TurnosApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfessionalsController : ControllerBase
{
    private readonly TurnosDbContext _context;

    public ProfessionalsController(TurnosDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtiene todos los profesionales (con filtro opcional por especialidad)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProfessionalResponse>>> GetAll([FromQuery] int? specialtyId)
    {
        var query = _context.Professionals
            .Include(p => p.User)
            .Include(p => p.Specialty)
            .Where(p => p.User.IsActive)
            .AsQueryable();

        if (specialtyId.HasValue)
        {
            query = query.Where(p => p.SpecialtyId == specialtyId.Value);
        }

        var professionals = await query
            .Select(p => new ProfessionalResponse
            {
                Id = p.Id,
                FirstName = p.User.FirstName,
                LastName = p.User.LastName,
                Email = p.User.Email,
                PhoneNumber = p.User.PhoneNumber,
                LicenseNumber = p.LicenseNumber,
                Specialty = new SpecialtyResponse
                {
                    Id = p.Specialty.Id,
                    Name = p.Specialty.Name,
                    Description = p.Specialty.Description,
                    DurationMinutes = p.Specialty.DurationMinutes
                }
            })
            .ToListAsync();

        return Ok(professionals);
    }

    /// <summary>
    /// Obtiene un profesional por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProfessionalResponse>> GetById(int id)
    {
        var professional = await _context.Professionals
            .Include(p => p.User)
            .Include(p => p.Specialty)
            .Where(p => p.Id == id)
            .Select(p => new ProfessionalResponse
            {
                Id = p.Id,
                FirstName = p.User.FirstName,
                LastName = p.User.LastName,
                Email = p.User.Email,
                PhoneNumber = p.User.PhoneNumber,
                LicenseNumber = p.LicenseNumber,
                Specialty = new SpecialtyResponse
                {
                    Id = p.Specialty.Id,
                    Name = p.Specialty.Name,
                    Description = p.Specialty.Description,
                    DurationMinutes = p.Specialty.DurationMinutes
                }
            })
            .FirstOrDefaultAsync();

        if (professional == null)
            return NotFound(new { message = "Profesional no encontrado" });

        return Ok(professional);
    }

    /// <summary>
    /// Obtiene los horarios disponibles de un profesional para una fecha específica
    /// </summary>
    [HttpGet("{id}/available-slots")]
    public async Task<ActionResult<IEnumerable<AvailableSlotResponse>>> GetAvailableSlots(
        int id, 
        [FromQuery] DateTime date)
    {
        var professional = await _context.Professionals
            .Include(p => p.Specialty)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (professional == null)
            return NotFound(new { message = "Profesional no encontrado" });

        // Obtener horarios del profesional para ese día de la semana
        var dayOfWeek = date.DayOfWeek;
        var schedules = await _context.Schedules
            .Where(s => s.ProfessionalId == id && s.DayOfWeek == dayOfWeek && s.IsActive)
            .ToListAsync();

        if (!schedules.Any())
            return Ok(new List<AvailableSlotResponse>());

        // Obtener turnos ya reservados para esa fecha
        var appointments = await _context.Appointments
            .Where(a => a.ProfessionalId == id 
                && a.DateTime.Date == date.Date
                && a.Status != Domain.Enums.AppointmentStatus.Canceled)
            .ToListAsync();

        var availableSlots = new List<AvailableSlotResponse>();

        foreach (var schedule in schedules)
        {
            var currentTime = date.Date.Add(schedule.StartTime);
            var endTime = date.Date.Add(schedule.EndTime);

            while (currentTime.Add(TimeSpan.FromMinutes(professional.Specialty.DurationMinutes)) <= endTime)
            {
                var isAvailable = !appointments.Any(a =>
                    a.DateTime < currentTime.Add(TimeSpan.FromMinutes(professional.Specialty.DurationMinutes))
                    && currentTime < a.DateTime.Add(TimeSpan.FromMinutes(a.DurationMinutes)));

                availableSlots.Add(new AvailableSlotResponse
                {
                    DateTime = currentTime,
                    DurationMinutes = professional.Specialty.DurationMinutes,
                    IsAvailable = isAvailable && currentTime > DateTime.Now
                });

                currentTime = currentTime.Add(TimeSpan.FromMinutes(professional.Specialty.DurationMinutes));
            }
        }

        return Ok(availableSlots.OrderBy(s => s.DateTime));
    }
}
