using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TurnosApp.Contracts.Responses;
using TurnosApp.Infrastructure.Data;

namespace TurnosApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpecialtiesController : ControllerBase
{
    private readonly TurnosDbContext _context;

    public SpecialtiesController(TurnosDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtiene todas las especialidades
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SpecialtyResponse>>> GetAll()
    {
        var specialties = await _context.Specialties
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
    /// Obtiene una especialidad por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<SpecialtyResponse>> GetById(int id)
    {
        var specialty = await _context.Specialties
            .Where(s => s.Id == id)
            .Select(s => new SpecialtyResponse
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                DurationMinutes = s.DurationMinutes
            })
            .FirstOrDefaultAsync();

        if (specialty == null)
            return NotFound(new { message = "Especialidad no encontrada" });

        return Ok(specialty);
    }
}
