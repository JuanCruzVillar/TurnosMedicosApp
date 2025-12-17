using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TurnosApp.Contracts.Requests;
using TurnosApp.Contracts.Responses;
using TurnosApp.Domain.Entities;
using TurnosApp.Domain.Enums;
using TurnosApp.Infrastructure.Data;

namespace TurnosApp.API.Controllers.Admin;

[ApiController]
[Route("api/Admin/[controller]")]
[Authorize(Roles = "Admin")]
public class ProfessionalsController : ControllerBase
{
    private readonly TurnosDbContext _context;

    public ProfessionalsController(TurnosDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtiene todos los profesionales con sus detalles
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProfessionalResponse>>> GetAll()
    {
        var professionals = await _context.Professionals
            .Include(p => p.User)
            .Include(p => p.Specialty)
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
    /// Crea un nuevo profesional
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ProfessionalResponse>> Create(CreateProfessionalRequest request)
    {
        // Validar que el email no esté registrado
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest(new { message = "El email ya está registrado" });
        }

        // Validar que la especialidad exista
        var specialty = await _context.Specialties.FindAsync(request.SpecialtyId);
        if (specialty == null)
        {
            return BadRequest(new { message = "Especialidad no encontrada" });
        }

        // Validar que la matrícula no esté registrada
        if (await _context.Professionals.AnyAsync(p => p.LicenseNumber == request.LicenseNumber))
        {
            return BadRequest(new { message = "La matrícula ya está registrada" });
        }

        // Crear usuario
        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Role = UserRole.Professional,
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Crear profesional
        var professional = new Professional
        {
            UserId = user.Id,
            LicenseNumber = request.LicenseNumber,
            SpecialtyId = request.SpecialtyId
        };

        _context.Professionals.Add(professional);
        await _context.SaveChangesAsync();

        // Cargar relaciones para la respuesta
        await _context.Entry(professional)
            .Reference(p => p.User)
            .LoadAsync();
        await _context.Entry(professional)
            .Reference(p => p.Specialty)
            .LoadAsync();

        var response = new ProfessionalResponse
        {
            Id = professional.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            LicenseNumber = professional.LicenseNumber,
            Specialty = new SpecialtyResponse
            {
                Id = specialty.Id,
                Name = specialty.Name,
                Description = specialty.Description,
                DurationMinutes = specialty.DurationMinutes
            }
        };

        return CreatedAtAction(nameof(GetById), new { id = professional.Id }, response);
    }

    /// <summary>
    /// Actualiza un profesional existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateProfessionalRequest request)
    {
        var professional = await _context.Professionals
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (professional == null)
            return NotFound(new { message = "Profesional no encontrado" });

        // Validar que la especialidad exista
        if (!await _context.Specialties.AnyAsync(s => s.Id == request.SpecialtyId))
        {
            return BadRequest(new { message = "Especialidad no encontrada" });
        }

        // Validar que la matrícula no esté registrada en otro profesional
        if (await _context.Professionals.AnyAsync(p => p.LicenseNumber == request.LicenseNumber && p.Id != id))
        {
            return BadRequest(new { message = "La matrícula ya está registrada en otro profesional" });
        }

        // Actualizar usuario
        professional.User.FirstName = request.FirstName;
        professional.User.LastName = request.LastName;
        professional.User.PhoneNumber = request.PhoneNumber;
        professional.User.IsActive = request.IsActive;
        professional.User.UpdatedAt = DateTime.UtcNow;

        // Actualizar profesional
        professional.LicenseNumber = request.LicenseNumber;
        professional.SpecialtyId = request.SpecialtyId;
        professional.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Profesional actualizado exitosamente" });
    }

    /// <summary>
    /// Elimina un profesional (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var professional = await _context.Professionals
            .Include(p => p.User)
            .Include(p => p.Appointments)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (professional == null)
            return NotFound(new { message = "Profesional no encontrado" });

        // Validar que no tenga turnos pendientes
        var hasPendingAppointments = professional.Appointments
            .Any(a => a.Status == Domain.Enums.AppointmentStatus.Scheduled || 
                     a.Status == Domain.Enums.AppointmentStatus.Confirmed);

        if (hasPendingAppointments)
        {
            return BadRequest(new { message = "No se puede eliminar un profesional con turnos pendientes" });
        }

        // Soft delete
        professional.IsDeleted = true;
        professional.UpdatedAt = DateTime.UtcNow;
        professional.User.IsActive = false;
        professional.User.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Profesional eliminado exitosamente" });
    }

    /// <summary>
    /// Activa o desactiva un profesional
    /// </summary>
    [HttpPatch("{id}/toggle-active")]
    public async Task<ActionResult> ToggleActive(int id)
    {
        var professional = await _context.Professionals
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (professional == null)
            return NotFound(new { message = "Profesional no encontrado" });

        professional.User.IsActive = !professional.User.IsActive;
        professional.User.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { 
            message = professional.User.IsActive ? "Profesional activado" : "Profesional desactivado",
            isActive = professional.User.IsActive
        });
    }
}
