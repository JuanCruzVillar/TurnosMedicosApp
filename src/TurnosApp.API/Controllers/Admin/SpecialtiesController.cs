using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TurnosApp.Contracts.Requests;
using TurnosApp.Contracts.Responses;
using TurnosApp.Domain.Entities;
using TurnosApp.Infrastructure.Data;

namespace TurnosApp.API.Controllers.Admin;

[ApiController]
[Route("api/Admin/[controller]")]
[Authorize(Roles = "Admin")]
public class SpecialtiesController : ControllerBase
{
    private readonly TurnosDbContext _context;

    public SpecialtiesController(TurnosDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtiene todas las especialidades (Admin)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SpecialtyResponse>>> GetAll()
    {
        var specialties = await _context.Specialties
            .Include(s => s.Professionals)
            .Select(s => new SpecialtyResponse
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                DurationMinutes = s.DurationMinutes
            })
            .ToListAsync();

        return Ok(specialties);
    }

    /// <summary>
    /// Crea una nueva especialidad
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SpecialtyResponse>> Create(CreateSpecialtyRequest request)
    {
        // Validar que no exista una especialidad con el mismo nombre
        if (await _context.Specialties.AnyAsync(s => s.Name == request.Name))
        {
            return BadRequest(new { message = "Ya existe una especialidad con ese nombre" });
        }

        var specialty = new Specialty
        {
            Name = request.Name,
            Description = request.Description,
            DurationMinutes = request.DurationMinutes
        };

        _context.Specialties.Add(specialty);
        await _context.SaveChangesAsync();

        var response = new SpecialtyResponse
        {
            Id = specialty.Id,
            Name = specialty.Name,
            Description = specialty.Description,
            DurationMinutes = specialty.DurationMinutes
        };

        return CreatedAtAction(nameof(GetAll), new { id = specialty.Id }, response);
    }

    /// <summary>
    /// Actualiza una especialidad existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateSpecialtyRequest request)
    {
        var specialty = await _context.Specialties.FindAsync(id);

        if (specialty == null)
            return NotFound(new { message = "Especialidad no encontrada" });

        // Validar que no exista otra especialidad con el mismo nombre
        if (await _context.Specialties.AnyAsync(s => s.Name == request.Name && s.Id != id))
        {
            return BadRequest(new { message = "Ya existe otra especialidad con ese nombre" });
        }

        specialty.Name = request.Name;
        specialty.Description = request.Description;
        specialty.DurationMinutes = request.DurationMinutes;
        specialty.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Especialidad actualizada exitosamente" });
    }

    /// <summary>
    /// Elimina una especialidad (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var specialty = await _context.Specialties
            .Include(s => s.Professionals)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (specialty == null)
            return NotFound(new { message = "Especialidad no encontrada" });

        // Validar que no tenga profesionales asignados
        if (specialty.Professionals.Any())
        {
            return BadRequest(new { message = "No se puede eliminar una especialidad con profesionales asignados" });
        }

        specialty.IsDeleted = true;
        specialty.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Especialidad eliminada exitosamente" });
    }
}
