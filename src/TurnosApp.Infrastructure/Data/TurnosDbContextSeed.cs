using Microsoft.EntityFrameworkCore;
using TurnosApp.Domain.Entities;
using TurnosApp.Domain.Enums;

namespace TurnosApp.Infrastructure.Data;

public static class TurnosDbContextSeed
{
    public static async Task SeedAsync(TurnosDbContext context)
    {
        if (await context.Users.AnyAsync())
            return; // Ya hay datos
        
        // Crear especialidades
        var cardiology = new Specialty
        {
            Name = "Cardiología",
            Description = "Especialidad médica que se encarga del estudio, diagnóstico y tratamiento de las enfermedades del corazón",
            DurationMinutes = 30
        };
        
        var dermatology = new Specialty
        {
            Name = "Dermatología",
            Description = "Especialidad médica que se ocupa del conocimiento y estudio de la piel humana",
            DurationMinutes = 20
        };
        
        var pediatrics = new Specialty
        {
            Name = "Pediatría",
            Description = "Rama de la medicina que se especializa en la salud y las enfermedades de los niños",
            DurationMinutes = 25
        };
        
        context.Specialties.AddRange(cardiology, dermatology, pediatrics);
        await context.SaveChangesAsync();
        
        // Crear usuario Admin
        var adminUser = new User
        {
            Email = "admin@turnosapp.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            FirstName = "Admin",
            LastName = "Sistema",
            PhoneNumber = "1234567890",
            Role = UserRole.Admin,
            IsActive = true
        };
        
        // Crear usuarios profesionales
        var drGomezUser = new User
        {
            Email = "dr.gomez@turnosapp.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor123!"),
            FirstName = "Carlos",
            LastName = "Gómez",
            PhoneNumber = "1145678901",
            Role = UserRole.Professional,
            IsActive = true
        };
        
        var draLopezUser = new User
        {
            Email = "dra.lopez@turnosapp.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor123!"),
            FirstName = "María",
            LastName = "López",
            PhoneNumber = "1156789012",
            Role = UserRole.Professional,
            IsActive = true
        };
        
        // Crear usuario paciente
        var patientUser = new User
        {
            Email = "paciente@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Paciente123!"),
            FirstName = "Juan",
            LastName = "Pérez",
            PhoneNumber = "1167890123",
            Role = UserRole.Patient,
            IsActive = true
        };
        
        context.Users.AddRange(adminUser, drGomezUser, draLopezUser, patientUser);
        await context.SaveChangesAsync();
        
        // Crear profesionales
        var drGomez = new Professional
        {
            UserId = drGomezUser.Id,
            LicenseNumber = "MN-123456",
            SpecialtyId = cardiology.Id
        };
        
        var draLopez = new Professional
        {
            UserId = draLopezUser.Id,
            LicenseNumber = "MN-789012",
            SpecialtyId = dermatology.Id
        };
        
        context.Professionals.AddRange(drGomez, draLopez);
        await context.SaveChangesAsync();
        
        // Crear horarios para Dr. Gómez (Lunes a Viernes 9-13hs)
        for (int day = 1; day <= 5; day++)
        {
            context.Schedules.Add(new Schedule
            {
                ProfessionalId = drGomez.Id,
                DayOfWeek = (DayOfWeek)day,
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(13, 0, 0),
                IsActive = true
            });
        }
        
        // Crear horarios para Dra. López (Lunes, Miércoles, Viernes 14-18hs)
        context.Schedules.Add(new Schedule
        {
            ProfessionalId = draLopez.Id,
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeSpan(14, 0, 0),
            EndTime = new TimeSpan(18, 0, 0),
            IsActive = true
        });
        
        context.Schedules.Add(new Schedule
        {
            ProfessionalId = draLopez.Id,
            DayOfWeek = DayOfWeek.Wednesday,
            StartTime = new TimeSpan(14, 0, 0),
            EndTime = new TimeSpan(18, 0, 0),
            IsActive = true
        });
        
        context.Schedules.Add(new Schedule
        {
            ProfessionalId = draLopez.Id,
            DayOfWeek = DayOfWeek.Friday,
            StartTime = new TimeSpan(14, 0, 0),
            EndTime = new TimeSpan(18, 0, 0),
            IsActive = true
        });
        
        await context.SaveChangesAsync();
        
        // Crear paciente
        var patient = new Patient
        {
            UserId = patientUser.Id,
            DocumentNumber = "12345678",
            DateOfBirth = new DateTime(1990, 5, 15),
            Address = "Av. Corrientes 1234, CABA",
            MedicalInsurance = "OSDE",
            InsuranceNumber = "123456789"
        };
        
        context.Patients.Add(patient);
        await context.SaveChangesAsync();
        
        Console.WriteLine("✅ Datos de prueba creados exitosamente");
    }
}
