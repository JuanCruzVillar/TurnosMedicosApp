using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TurnosApp.Application.Common;
using TurnosApp.Contracts.Requests;
using TurnosApp.Contracts.Responses;
using TurnosApp.Domain.Entities;
using TurnosApp.Domain.Enums;
using TurnosApp.Infrastructure.Data;

namespace TurnosApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly TurnosDbContext _context;
    private readonly IJwtService _jwtService;

    public AuthController(TurnosDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        // Verificar si el email ya existe
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest(new { message = "El email ya está registrado" });
        }

        // Crear nuevo usuario
        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Role = UserRole.Patient, // Por defecto es paciente
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Crear el perfil de paciente
        var patient = new Patient
        {
            UserId = user.Id
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        // Generar token
        var token = _jwtService.GenerateToken(user);

        return Ok(new AuthResponse
        {
            Token = token,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role.ToString()
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        // Buscar usuario por email
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Email o contraseña incorrectos" });
        }

        if (!user.IsActive)
        {
            return Unauthorized(new { message = "Usuario inactivo" });
        }

        // Generar token
        var token = _jwtService.GenerateToken(user);

        return Ok(new AuthResponse
        {
            Token = token,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role.ToString()
        });
    }
}
