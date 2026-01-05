using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TurnosApp.API.Common;
using TurnosApp.Contracts.Requests;
using TurnosApp.Contracts.Responses;
using TurnosApp.Domain.Constants;
using TurnosApp.Domain.Entities;
using TurnosApp.Domain.Enums;
using TurnosApp.Infrastructure.Data;

namespace TurnosApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : BaseController
{
    private readonly TurnosDbContext _context;
    private readonly ILogger<AppointmentsController> _logger;

    public AppointmentsController(
        TurnosDbContext context,
        ILogger<AppointmentsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene los turnos del usuario autenticado (con paginación)
    /// </summary>
    [HttpGet("my-appointments")]
    public async Task<ActionResult<PagedResponse<AppointmentResponse>>> GetMyAppointments(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null)
    {
        var (patient, professional) = await GetUserProfilesAsync();

        if (patient == null && professional == null)
            return NotFound(new { message = "Perfil de usuario no encontrado" });

        IQueryable<Appointment> query = _context.Appointments
            .Include(a => a.Professional).ThenInclude(p => p.User)
            .Include(a => a.Professional).ThenInclude(p => p.Specialty)
            .Include(a => a.Patient).ThenInclude(p => p.User);

        if (patient != null)
            query = query.Where(a => a.PatientId == patient.Id);
        else if (professional != null)
            query = query.Where(a => a.ProfessionalId == professional.Id);

        // Filtro opcional por estado
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<AppointmentStatus>(status, out var statusEnum))
            query = query.Where(a => a.Status == statusEnum);

        var totalItems = await query.CountAsync();

        var appointments = await query
            .OrderByDescending(a => a.DateTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var response = new PagedResponse<AppointmentResponse>
        {
            Items = appointments.Select(a => a.ToResponse()),
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        };

        return Ok(response);
    }

    /// <summary>
    /// Obtiene los próximos turnos del usuario (útil para dashboard)
    /// </summary>
    [HttpGet("upcoming")]
    public async Task<ActionResult<IEnumerable<AppointmentResponse>>> GetUpcomingAppointments(
        [FromQuery] int limit = 5)
    {
        var (patient, professional) = await GetUserProfilesAsync();

        if (patient == null && professional == null)
            return NotFound(new { message = "Perfil de usuario no encontrado" });

        IQueryable<Appointment> query = _context.Appointments
            .Include(a => a.Professional).ThenInclude(p => p.User)
            .Include(a => a.Professional).ThenInclude(p => p.Specialty)
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .Where(a => a.DateTime > DateTime.UtcNow
                        && a.Status != AppointmentStatus.Canceled);

        if (patient != null)
            query = query.Where(a => a.PatientId == patient.Id);
        else if (professional != null)
            query = query.Where(a => a.ProfessionalId == professional.Id);

        var appointments = await query
            .OrderBy(a => a.DateTime)
            .Take(limit)
            .ToListAsync();

        return Ok(appointments.Select(a => a.ToResponse()));
    }

    /// <summary>
    /// Obtiene un turno por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentResponse>> GetById(int id)
    {
        var (patient, professional) = await GetUserProfilesAsync();

        if (patient == null && professional == null)
            return NotFound(new { message = "Perfil de usuario no encontrado" });

        var appointment = await _context.Appointments
            .Include(a => a.Professional).ThenInclude(p => p.User)
            .Include(a => a.Professional).ThenInclude(p => p.Specialty)
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment == null)
            return NotFound(new { message = "Turno no encontrado" });

        // Validar permisos
        if (patient != null && appointment.PatientId != patient.Id)
            return Forbid();

        if (professional != null && appointment.ProfessionalId != professional.Id)
            return Forbid();

        return Ok(appointment.ToResponse());
    }

    /// <summary>
    /// Crea un nuevo turno (solo pacientes)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = Roles.Patient)]
    [ProducesResponseType(typeof(AppointmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppointmentResponse>> Create(CreateAppointmentRequest request)
    {
        var validationResult = ValidateModelState();
        if (validationResult != null) return validationResult;

        var (patient, _) = await GetUserProfilesAsync();
        if (patient == null)
        {
            _logger.LogWarning("Perfil de paciente no encontrado para el usuario {UserId}", GetUserId());
            return BadRequest(new { message = "No se encontró el perfil de paciente" });
        }

        var professional = await _context.Professionals
            .Include(p => p.Specialty)
            .FirstOrDefaultAsync(p => p.Id == request.ProfessionalId);

        if (professional == null)
            return NotFound(new { message = "Profesional no encontrado" });

        // Validar fecha futura (con buffer de 15 minutos)
        if (request.DateTime.ToUniversalTime() <= DateTime.UtcNow.AddMinutes(15))
            return BadRequest(new { message = "La fecha debe ser al menos 15 minutos en el futuro" });

        // Validar que el profesional atienda ese día
        var dayOfWeek = request.DateTime.DayOfWeek;
        var schedule = await _context.Schedules
            .FirstOrDefaultAsync(s =>
                s.ProfessionalId == request.ProfessionalId
                && s.DayOfWeek == dayOfWeek
                && s.IsActive);

        if (schedule == null)
            return BadRequest(new { message = "El profesional no atiende ese día" });

        // Validar horario dentro del rango de atención
        var timeOfDay = request.DateTime.TimeOfDay;
        var duration = TimeSpan.FromMinutes(professional.Specialty.DurationMinutes);

        if (timeOfDay < schedule.StartTime || timeOfDay.Add(duration) > schedule.EndTime)
            return BadRequest(new { message = "El horario está fuera del rango de atención" });

        // Validar overlapping de turnos (OPTIMIZADO)
        var requestEnd = request.DateTime.AddMinutes(professional.Specialty.DurationMinutes);

        var conflictingAppointments = await _context.Appointments
            .Where(a =>
                a.ProfessionalId == request.ProfessionalId
                && a.Status != AppointmentStatus.Canceled
                && a.DateTime.Date == request.DateTime.Date
                && a.DateTime < requestEnd) // Pre-filtro en DB
            .Select(a => new { a.DateTime, a.DurationMinutes })
            .ToListAsync();

        var hasConflict = conflictingAppointments.Any(a =>
        {
            var existingEnd = a.DateTime.AddMinutes(a.DurationMinutes);
            return request.DateTime < existingEnd;
        });

        if (hasConflict)
            return BadRequest(new { message = "El horario ya está ocupado" });

        // Crear turno
        var appointment = new Appointment
        {
            ProfessionalId = request.ProfessionalId,
            PatientId = patient.Id,
            DateTime = request.DateTime,
            DurationMinutes = professional.Specialty.DurationMinutes,
            Status = AppointmentStatus.Scheduled,
            Reason = request.Reason
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        // Cargar relaciones para el response
        await _context.Entry(appointment)
            .Reference(a => a.Professional)
            .Query()
            .Include(p => p.User)
            .Include(p => p.Specialty)
            .LoadAsync();

        await _context.Entry(appointment)
            .Reference(a => a.Patient)
            .Query()
            .Include(p => p.User)
            .LoadAsync();

        return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment.ToResponse());
    }

    /// <summary>
    /// Cancela un turno
    /// </summary>
    [HttpPost("{id}/cancel")]
    public async Task<ActionResult> Cancel(int id, CancelAppointmentRequest request)
    {
        var (patient, professional) = await GetUserProfilesAsync();

        if (patient == null && professional == null)
            return NotFound(new { message = "Perfil de usuario no encontrado" });

        var appointment = await _context.Appointments.FindAsync(id);

        if (appointment == null)
            return NotFound(new { message = "Turno no encontrado" });

        // Validar permisos
        if (patient != null && appointment.PatientId != patient.Id)
            return Forbid();

        if (professional != null && appointment.ProfessionalId != professional.Id)
            return Forbid();

        // Validar estado
        if (appointment.Status == AppointmentStatus.Canceled)
            return BadRequest(new { message = "El turno ya está cancelado" });

        if (appointment.Status == AppointmentStatus.Completed)
            return BadRequest(new { message = "No se puede cancelar un turno completado" });

        appointment.Status = AppointmentStatus.Canceled;
        appointment.CanceledAt = DateTime.UtcNow;
        appointment.CancellationReason = request.Reason;
        appointment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Turno cancelado exitosamente" });
    }

    /// <summary>
    /// Actualiza el estado de un turno (solo profesionales y admins)
    /// </summary>
    [HttpPatch("{id}/status")]
    [Authorize(Roles = $"{Roles.Professional},{Roles.Admin}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateStatus(int id, UpdateAppointmentStatusRequest request)
    {
        var userRole = GetUserRole();

        var appointment = await _context.Appointments.FindAsync(id);

        if (appointment == null)
        {
            _logger.LogWarning("Intento de actualizar turno inexistente: {AppointmentId}", id);
            return NotFound(new { message = "Turno no encontrado" });
        }

        // Si es profesional, validar que sea su turno
        if (userRole == Roles.Professional)
        {
            var (_, professional) = await GetUserProfilesAsync();
            if (professional == null || appointment.ProfessionalId != professional.Id)
                return Forbid();
        }

        appointment.Status = request.Status;
        appointment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Estado actualizado exitosamente" });
    }

    /// <summary>
    /// Actualiza las notas de un turno (solo profesionales)
    /// </summary>
    [HttpPatch("{id}/notes")]
    [Authorize(Roles = Roles.Professional)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateNotes(int id, UpdateAppointmentNotesRequest request)
    {
        var (_, professional) = await GetUserProfilesAsync();

        if (professional == null)
            return NotFound(new { message = "Profesional no encontrado" });

        var appointment = await _context.Appointments.FindAsync(id);

        if (appointment == null)
            return NotFound(new { message = "Turno no encontrado" });

        if (appointment.ProfessionalId != professional.Id)
            return Forbid();

        appointment.Notes = request.Notes;
        appointment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Notas actualizadas exitosamente" });
    }

    // ==================== MÉTODOS PRIVADOS ====================

    /// <summary>
    /// Obtiene el perfil del usuario autenticado (Patient o Professional)
    /// </summary>
    private async Task<(Patient? patient, Professional? professional)> GetUserProfilesAsync()
    {
        var userId = GetUserId();
        var userRole = GetUserRole();

        if (userRole == Roles.Patient)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            return (patient, null);
        }
        else if (userRole == Roles.Professional)
        {
            var professional = await _context.Professionals.FirstOrDefaultAsync(p => p.UserId == userId);
            return (null, professional);
        }

        return (null, null);
    }
}

// ==================== EXTENSIONES ====================

public static class AppointmentExtensions
{
    public static AppointmentResponse ToResponse(this Appointment appointment)
    {
        return new AppointmentResponse
        {
            Id = appointment.Id,
            DateTime = appointment.DateTime,
            DurationMinutes = appointment.DurationMinutes,
            Status = appointment.Status.ToString(),
            Reason = appointment.Reason,
            Notes = appointment.Notes,
            Professional = new ProfessionalResponse
            {
                Id = appointment.Professional.Id,
                FirstName = appointment.Professional.User.FirstName,
                LastName = appointment.Professional.User.LastName,
                Email = appointment.Professional.User.Email,
                PhoneNumber = appointment.Professional.User.PhoneNumber,
                LicenseNumber = appointment.Professional.LicenseNumber,
                Specialty = new SpecialtyResponse
                {
                    Id = appointment.Professional.Specialty.Id,
                    Name = appointment.Professional.Specialty.Name,
                    Description = appointment.Professional.Specialty.Description,
                    DurationMinutes = appointment.Professional.Specialty.DurationMinutes
                }
            },
            Patient = new PatientResponse
            {
                Id = appointment.Patient.Id,
                FirstName = appointment.Patient.User.FirstName,
                LastName = appointment.Patient.User.LastName,
                Email = appointment.Patient.User.Email,
                PhoneNumber = appointment.Patient.User.PhoneNumber
            }
        };
    }
}