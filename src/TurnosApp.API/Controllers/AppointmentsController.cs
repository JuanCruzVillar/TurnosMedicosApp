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

    [HttpGet("my-appointments")]
    public async Task<ActionResult<IEnumerable<AppointmentResponse>>> GetMyAppointments()
    {
        var userId = GetUserId();
        var userRole = GetUserRole();

        IQueryable<Appointment> query = _context.Appointments
            .Include(a => a.Professional).ThenInclude(p => p.User)
            .Include(a => a.Professional).ThenInclude(p => p.Specialty)
            .Include(a => a.Patient).ThenInclude(p => p.User);

        if (userRole == Roles.Patient)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null)
            {
                _logger.LogWarning("Paciente no encontrado para el usuario {UserId}", userId);
                return NotFound(new { message = "Paciente no encontrado" });
            }

            query = query.Where(a => a.PatientId == patient.Id);
        }
        else if (userRole == Roles.Professional)
        {
            var professional = await _context.Professionals.FirstOrDefaultAsync(p => p.UserId == userId);
            if (professional == null)
            {
                _logger.LogWarning("Profesional no encontrado para el usuario {UserId}", userId);
                return NotFound(new { message = "Profesional no encontrado" });
            }

            query = query.Where(a => a.ProfessionalId == professional.Id);
        }

        var appointments = await query
            .OrderByDescending(a => a.DateTime)
            .Select(a => new AppointmentResponse
            {
                Id = a.Id,
                DateTime = a.DateTime,
                DurationMinutes = a.DurationMinutes,
                Status = a.Status.ToString(),
                Reason = a.Reason,
                Notes = a.Notes,
                Professional = new ProfessionalResponse
                {
                    Id = a.Professional.Id,
                    FirstName = a.Professional.User.FirstName,
                    LastName = a.Professional.User.LastName,
                    Email = a.Professional.User.Email,
                    PhoneNumber = a.Professional.User.PhoneNumber,
                    LicenseNumber = a.Professional.LicenseNumber,
                    Specialty = new SpecialtyResponse
                    {
                        Id = a.Professional.Specialty.Id,
                        Name = a.Professional.Specialty.Name,
                        Description = a.Professional.Specialty.Description,
                        DurationMinutes = a.Professional.Specialty.DurationMinutes
                    }
                },
                Patient = new PatientResponse
                {
                    Id = a.Patient.Id,
                    FirstName = a.Patient.User.FirstName,
                    LastName = a.Patient.User.LastName,
                    Email = a.Patient.User.Email,
                    PhoneNumber = a.Patient.User.PhoneNumber
                }
            })
            .ToListAsync();

        return Ok(appointments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentResponse>> GetById(int id)
    {
        var userId = GetUserId();
        var userRole = GetUserRole();

        var appointment = await _context.Appointments
            .Include(a => a.Professional).ThenInclude(p => p.User)
            .Include(a => a.Professional).ThenInclude(p => p.Specialty)
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment == null)
            return NotFound(new { message = "Turno no encontrado" });

        if (userRole == Roles.Patient)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null || appointment.PatientId != patient.Id)
                return Forbid();
        }
        else if (userRole == Roles.Professional)
        {
            var professional = await _context.Professionals.FirstOrDefaultAsync(p => p.UserId == userId);
            if (professional == null || appointment.ProfessionalId != professional.Id)
                return Forbid();
        }

        var response = new AppointmentResponse
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

        return Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Patient)]
    [ProducesResponseType(typeof(AppointmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppointmentResponse>> Create(CreateAppointmentRequest request)
    {
        var validationResult = ValidateModelState();
        if (validationResult != null) return validationResult;

        var userId = GetUserId();

        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
        if (patient == null)
        {
            _logger.LogWarning("Perfil de paciente no encontrado para el usuario {UserId}", userId);
            return BadRequest(new { message = "No se encontró el perfil de paciente" });
        }

        var professional = await _context.Professionals
            .Include(p => p.Specialty)
            .FirstOrDefaultAsync(p => p.Id == request.ProfessionalId);

        if (professional == null)
            return NotFound(new { message = "Profesional no encontrado" });

        if (request.DateTime <= DateTime.Now)
            return BadRequest(new { message = "La fecha debe ser futura" });

        var dayOfWeek = request.DateTime.DayOfWeek;
        var schedule = await _context.Schedules
            .FirstOrDefaultAsync(s => 
                s.ProfessionalId == request.ProfessionalId 
                && s.DayOfWeek == dayOfWeek 
                && s.IsActive);

        if (schedule == null)
            return BadRequest(new { message = "El profesional no atiende ese día" });

        var timeOfDay = request.DateTime.TimeOfDay;
        var duration = TimeSpan.FromMinutes(professional.Specialty.DurationMinutes);

        if (timeOfDay < schedule.StartTime || timeOfDay.Add(duration) > schedule.EndTime)
            return BadRequest(new { message = "El horario está fuera del rango de atención" });

        // Validar overlapping - CORREGIDO: traer a memoria primero
        var requestEnd = request.DateTime.AddMinutes(professional.Specialty.DurationMinutes);
        
        var existingAppointments = await _context.Appointments
            .Where(a => 
                a.ProfessionalId == request.ProfessionalId
                && a.Status != AppointmentStatus.Canceled
                && a.DateTime.Date == request.DateTime.Date)
            .ToListAsync(); // Traer a memoria

        // Validar en memoria
        var hasConflict = existingAppointments.Any(a =>
        {
            var existingEnd = a.DateTime.AddMinutes(a.DurationMinutes);
            return a.DateTime < requestEnd && request.DateTime < existingEnd;
        });

        if (hasConflict)
            return BadRequest(new { message = "El horario ya está ocupado" });

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

        var response = new AppointmentResponse
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

        return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, response);
    }

    [HttpPost("{id}/cancel")]
    public async Task<ActionResult> Cancel(int id, CancelAppointmentRequest request)
    {
        var userId = GetUserId();
        var userRole = GetUserRole();

        var appointment = await _context.Appointments
            .Include(a => a.Professional)
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment == null)
            return NotFound(new { message = "Turno no encontrado" });

        if (userRole == Roles.Patient)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null || appointment.PatientId != patient.Id)
                return Forbid();
        }
        else if (userRole == Roles.Professional)
        {
            var professional = await _context.Professionals.FirstOrDefaultAsync(p => p.UserId == userId);
            if (professional == null || appointment.ProfessionalId != professional.Id)
                return Forbid();
        }

        if (appointment.Status == AppointmentStatus.Canceled)
            return BadRequest(new { message = "El turno ya está cancelado" });

        if (appointment.Status == AppointmentStatus.Completed)
            return BadRequest(new { message = "No se puede cancelar un turno completado" });

        appointment.Status = AppointmentStatus.Canceled;
        appointment.CanceledAt = DateTime.UtcNow;
        appointment.CancellationReason = request.Reason;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Turno cancelado exitosamente" });
    }

    [HttpPatch("{id}/status")]
    [Authorize(Roles = $"{Roles.Professional},{Roles.Admin}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        var userId = GetUserId();
        var userRole = GetUserRole();

        var appointment = await _context.Appointments.FindAsync(id);

        if (appointment == null)
        {
            _logger.LogWarning("Intento de actualizar turno inexistente: {AppointmentId}", id);
            return NotFound(new { message = "Turno no encontrado" });
        }

        if (userRole == Roles.Professional)
        {
            var professional = await _context.Professionals.FirstOrDefaultAsync(p => p.UserId == userId);
            if (professional == null || appointment.ProfessionalId != professional.Id)
                return Forbid();
        }

        if (!Enum.TryParse<AppointmentStatus>(status, out var newStatus))
            return BadRequest(new { message = "Estado inválido" });

        appointment.Status = newStatus;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Estado actualizado exitosamente" });
    }

    [HttpPatch("{id}/notes")]
    [Authorize(Roles = Roles.Professional)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateNotes(int id, [FromBody] UpdateAppointmentNotesRequest request)
    {
        var userId = GetUserId();
        var professional = await _context.Professionals.FirstOrDefaultAsync(p => p.UserId == userId);

        if (professional == null)
            return NotFound(new { message = "Profesional no encontrado" });

        var appointment = await _context.Appointments.FindAsync(id);

        if (appointment == null)
            return NotFound(new { message = "Turno no encontrado" });

        if (appointment.ProfessionalId != professional.Id)
            return Forbid("No tienes permiso para actualizar este turno");

        appointment.Notes = request.Notes;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Notas actualizadas exitosamente" });
    }

}
