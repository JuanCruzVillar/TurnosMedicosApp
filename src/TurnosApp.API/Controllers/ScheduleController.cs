using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TurnosApp.API.Common;
using TurnosApp.Contracts.Requests;
using TurnosApp.Contracts.Responses;
using TurnosApp.Domain.Constants;
using TurnosApp.Domain.Entities;
using TurnosApp.Infrastructure.Data;

namespace TurnosApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Professional)]
public class ScheduleController : BaseController
{
    private readonly TurnosDbContext _context;
    private readonly ILogger<ScheduleController> _logger;

    public ScheduleController(
        TurnosDbContext context,
        ILogger<ScheduleController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("my-schedule")]
    public async Task<ActionResult<IEnumerable<ScheduleResponse>>> GetMySchedule()
    {
        var userId = GetUserId();
        var professional = await _context.Professionals
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (professional == null)
            return NotFound(new { message = "Profesional no encontrado" });

        var schedules = await _context.Schedules
            .Where(s => s.ProfessionalId == professional.Id)
            .OrderBy(s => s.DayOfWeek)
            .Select(s => new ScheduleResponse
            {
                Id = s.Id,
                ProfessionalId = s.ProfessionalId,
                DayOfWeek = s.DayOfWeek,
                StartTime = s.StartTime.ToString(@"hh\:mm"),
                EndTime = s.EndTime.ToString(@"hh\:mm"),
                IsActive = s.IsActive
            })
            .ToListAsync();

        return Ok(schedules);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ScheduleResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ScheduleResponse>> Create(CreateScheduleRequest request)
    {
        var validationResult = ValidateModelState();
        if (validationResult != null) return validationResult;

        var userId = GetUserId();
        var professional = await _context.Professionals
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (professional == null)
        {
            _logger.LogWarning("Profesional no encontrado para el usuario {UserId}", userId);
            return NotFound(new { message = "Profesional no encontrado" });
        }

        // Verificar si ya existe un schedule para este día
        var existingSchedule = await _context.Schedules
            .FirstOrDefaultAsync(s => 
                s.ProfessionalId == professional.Id && 
                s.DayOfWeek == request.DayOfWeek);

        if (existingSchedule != null)
            return BadRequest(new { message = "Ya existe un horario para este día. Use PUT para actualizarlo." });

        if (!TimeSpan.TryParse(request.StartTime, out var startTime))
            return BadRequest(new { message = "Formato de hora de inicio inválido. Use formato HH:mm" });

        if (!TimeSpan.TryParse(request.EndTime, out var endTime))
            return BadRequest(new { message = "Formato de hora de fin inválido. Use formato HH:mm" });

        if (startTime >= endTime)
            return BadRequest(new { message = "La hora de inicio debe ser menor que la hora de fin" });

        var schedule = new Schedule
        {
            ProfessionalId = professional.Id,
            DayOfWeek = request.DayOfWeek,
            StartTime = startTime,
            EndTime = endTime,
            IsActive = request.IsActive
        };

        _context.Schedules.Add(schedule);
        await _context.SaveChangesAsync();

        var response = new ScheduleResponse
        {
            Id = schedule.Id,
            ProfessionalId = schedule.ProfessionalId,
            DayOfWeek = schedule.DayOfWeek,
            StartTime = schedule.StartTime.ToString(@"hh\:mm"),
            EndTime = schedule.EndTime.ToString(@"hh\:mm"),
            IsActive = schedule.IsActive
        };

        return CreatedAtAction(nameof(GetMySchedule), null, response);
    }

    [HttpPut]
    [ProducesResponseType(typeof(ScheduleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ScheduleResponse>> Update(UpdateScheduleRequest request)
    {
        var validationResult = ValidateModelState();
        if (validationResult != null) return validationResult;

        var userId = GetUserId();
        var professional = await _context.Professionals
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (professional == null)
        {
            _logger.LogWarning("Profesional no encontrado para el usuario {UserId}", userId);
            return NotFound(new { message = "Profesional no encontrado" });
        }

        var schedule = await _context.Schedules
            .FirstOrDefaultAsync(s => 
                s.Id == request.Id && 
                s.ProfessionalId == professional.Id);

        if (schedule == null)
            return NotFound(new { message = "Horario no encontrado" });

        if (!TimeSpan.TryParse(request.StartTime, out var startTime))
            return BadRequest(new { message = "Formato de hora de inicio inválido. Use formato HH:mm" });

        if (!TimeSpan.TryParse(request.EndTime, out var endTime))
            return BadRequest(new { message = "Formato de hora de fin inválido. Use formato HH:mm" });

        if (startTime >= endTime)
            return BadRequest(new { message = "La hora de inicio debe ser menor que la hora de fin" });

        schedule.DayOfWeek = request.DayOfWeek;
        schedule.StartTime = startTime;
        schedule.EndTime = endTime;
        schedule.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        var response = new ScheduleResponse
        {
            Id = schedule.Id,
            ProfessionalId = schedule.ProfessionalId,
            DayOfWeek = schedule.DayOfWeek,
            StartTime = schedule.StartTime.ToString(@"hh\:mm"),
            EndTime = schedule.EndTime.ToString(@"hh\:mm"),
            IsActive = schedule.IsActive
        };

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var userId = GetUserId();
        var professional = await _context.Professionals
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (professional == null)
            return NotFound(new { message = "Profesional no encontrado" });

        var schedule = await _context.Schedules
            .FirstOrDefaultAsync(s => 
                s.Id == id && 
                s.ProfessionalId == professional.Id);

        if (schedule == null)
            return NotFound(new { message = "Horario no encontrado" });

        _context.Schedules.Remove(schedule);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Horario eliminado exitosamente" });
    }

}

