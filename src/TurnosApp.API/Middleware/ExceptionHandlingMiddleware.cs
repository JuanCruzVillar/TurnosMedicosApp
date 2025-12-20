using System.Net;
using System.Text.Json;

namespace TurnosApp.API.Middleware;

/// <summary>
/// Middleware para manejo centralizado de excepciones.
/// Implementa el patrón de "catch-all" para capturar excepciones no manejadas
/// y retornar respuestas HTTP apropiadas. Esto evita exponer detalles internos
/// del sistema y proporciona una experiencia consistente al cliente.
/// 
/// Debe registrarse ANTES de UseAuthentication/UseAuthorization en Program.cs
/// para capturar errores de autenticación también.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    // Inyección de dependencias: RequestDelegate representa el siguiente middleware en la pipeline
    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Intercepta todas las requests y captura excepciones no manejadas.
    /// Usa structured logging para facilitar debugging en producción.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Structured logging: permite filtrar y buscar errores en logs
            _logger.LogError(ex, "Error no manejado: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Mapea excepciones a códigos HTTP apropiados.
    /// Implementa el principio de "fail fast" con mensajes claros.
    /// </summary>
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var message = "Ha ocurrido un error interno del servidor";

        // Pattern matching: mapea tipos de excepción a códigos HTTP estándar
        // Esto sigue las convenciones REST: 401 Unauthorized, 400 BadRequest, 404 NotFound
        if (exception is UnauthorizedAccessException)
        {
            code = HttpStatusCode.Unauthorized;
            message = exception.Message;
        }
        else if (exception is ArgumentException || exception is ArgumentNullException)
        {
            // 400 BadRequest: errores de validación o parámetros inválidos
            code = HttpStatusCode.BadRequest;
            message = exception.Message;
        }
        else if (exception is KeyNotFoundException)
        {
            // 404 NotFound: recurso no encontrado
            code = HttpStatusCode.NotFound;
            message = exception.Message;
        }

        var result = JsonSerializer.Serialize(new { message, statusCode = (int)code });
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        return context.Response.WriteAsync(result);
    }
}
