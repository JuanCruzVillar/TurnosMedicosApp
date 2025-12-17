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
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly TurnosDbContext _context;

    public ProfileController(TurnosDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<UserProfileResponse>> GetMyProfile()
    {
        var userId = GetUserId();
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            return NotFound(new { message = "Usuario no encontrado" });

        var response = new UserProfileResponse
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber
        };

        return Ok(response);
    }

    [HttpPut]
    public async Task<ActionResult<UserProfileResponse>> UpdateProfile(UpdateProfileRequest request)
    {
        var userId = GetUserId();
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            return NotFound(new { message = "Usuario no encontrado" });

        if (string.IsNullOrWhiteSpace(request.FirstName))
            return BadRequest(new { message = "El nombre es requerido" });

        if (string.IsNullOrWhiteSpace(request.LastName))
            return BadRequest(new { message = "El apellido es requerido" });

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;

        await _context.SaveChangesAsync();

        var response = new UserProfileResponse
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber
        };

        return Ok(response);
    }

    [HttpPost("change-password")]
    public async Task<ActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var userId = GetUserId();
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            return NotFound(new { message = "Usuario no encontrado" });

        if (string.IsNullOrWhiteSpace(request.CurrentPassword))
            return BadRequest(new { message = "La contraseña actual es requerida" });

        if (string.IsNullOrWhiteSpace(request.NewPassword))
            return BadRequest(new { message = "La nueva contraseña es requerida" });

        if (request.NewPassword != request.ConfirmPassword)
            return BadRequest(new { message = "Las contraseñas no coinciden" });

        if (request.NewPassword.Length < 6)
            return BadRequest(new { message = "La nueva contraseña debe tener al menos 6 caracteres" });

        // Verificar contraseña actual
        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            return BadRequest(new { message = "La contraseña actual es incorrecta" });

        // Actualizar contraseña
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Contraseña actualizada exitosamente" });
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }
}

