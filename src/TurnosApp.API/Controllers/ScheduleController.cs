using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TurnosApp.Contracts.Requests;
using TurnosApp.Contracts.Responses;
using TurnosApp.Domain.Entities;
using TurnosApp.Infrastructure.Data;

namespace TurnosApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Professional")]
public class ScheduleController : ControllerBase
{
    private readonly TurnosDbContext _context;

    public ScheduleController(TurnosDbContext context)
    {
        _context = context;
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
    public async Task<ActionResult<ScheduleResponse>> Create(CreateScheduleRequest request)
    {
        var userId = GetUserId();
        var professional = await _context.Professionals
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (professional == null)
            return NotFound(new { message = "Profesional no encontrado" });

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
    public async Task<ActionResult<ScheduleResponse>> Update(UpdateScheduleRequest request)
    {
        var userId = GetUserId();
        var professional = await _context.Professionals
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (professional == null)
            return NotFound(new { message = "Profesional no encontrado" });

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

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }
}

