using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TurnosApp.Application.Common;
using TurnosApp.Contracts.Requests;
using TurnosApp.Contracts.Responses;
using TurnosApp.Domain.Entities;
using TurnosApp.Domain.Enums;
using TurnosApp.Infrastructure.Data;

namespace TurnosApp.API.Controllers;

/// <summary>
/// Controlador de autenticación que maneja registro y login de usuarios.
/// Implementa inyección de dependencias para DbContext y JWT Service,
/// siguiendo el principio de inversión de dependencias (DIP).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly TurnosDbContext _context;
    private readonly IJwtService _jwtService;

    // Inyección de dependencias: permite testear fácilmente y seguir SOLID principles
    public AuthController(TurnosDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    /// <summary>
    /// Registra un nuevo usuario (paciente) en el sistema.
    /// Utiliza BCrypt para hashing de contraseñas (más seguro que SHA).
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        // Validación de negocio: evitar duplicados antes de intentar insertar
        // Usa AnyAsync para eficiencia (no carga toda la entidad)
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest(new { message = "El email ya está registrado" });
        }

        // Crear nuevo usuario con contraseña hasheada
        // BCrypt incluye salt automáticamente, mejor que MD5/SHA para passwords
        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Role = UserRole.Patient, // Por defecto es paciente (principio de menor privilegio)
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

    /// <summary>
    /// Autentica un usuario y genera un JWT token.
    /// Implementa verificación segura de contraseñas con BCrypt.Verify
    /// que es resistente a timing attacks.
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        // Buscar usuario por email (índice recomendado en Email para performance)
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        // Verificación segura: BCrypt.Verify es constante-time, previene timing attacks
        // Mensaje genérico para no revelar si el email existe o no (security best practice)
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Email o contraseña incorrectos" });
        }

        // Validación de negocio: usuarios inactivos no pueden autenticarse
        if (!user.IsActive)
        {
            return Unauthorized(new { message = "Usuario inactivo" });
        }

        // Generar JWT token stateless (no requiere sesión en servidor)
        // El token incluye claims del usuario (email, role) para autorización
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
