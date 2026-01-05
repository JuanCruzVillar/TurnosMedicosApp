using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using TurnosApp.Domain.Enums;

namespace TurnosApp.Contracts.Requests;

public record UpdateAppointmentStatusRequest
{
    [Required(ErrorMessage = "El estado es requerido")]
    public AppointmentStatus Status { get; init; }
}
