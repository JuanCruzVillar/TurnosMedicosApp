using TurnosApp.Domain.Entities;

namespace TurnosApp.Application.Common;

public interface IJwtService
{
    string GenerateToken(User user);
}
