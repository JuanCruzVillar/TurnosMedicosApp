using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace TurnosApp.API.Common;

/// <summary>
/// Controlador base con métodos comunes para obtener información del usuario autenticado
/// </summary>
[ApiController]
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// Obtiene el ID del usuario autenticado desde los claims
    /// </summary>
    protected int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Usuario no autenticado o token inválido");
        }
        return userId;
    }

    /// <summary>
    /// Obtiene el rol del usuario autenticado desde los claims
    /// </summary>
    protected string GetUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }

    /// <summary>
    /// Valida que el ModelState sea válido y retorna BadRequest si no lo es
    /// </summary>
    protected ActionResult? ValidateModelState()
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                message = "Datos de entrada inválidos",
                errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
            });
        }
        return null;
    }
}

