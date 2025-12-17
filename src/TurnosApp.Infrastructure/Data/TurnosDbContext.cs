using Microsoft.EntityFrameworkCore;
using TurnosApp.Domain.Entities;

namespace TurnosApp.Infrastructure.Data;

public class TurnosDbContext : DbContext
{
    public TurnosDbContext(DbContextOptions<TurnosDbContext> options) 
        : base(options)
    {
    }
    
    public DbSet<User> Users => Set<User>();
    public DbSet<Specialty> Specialties => Set<Specialty>();
    public DbSet<Professional> Professionals => Set<Professional>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Aplicar todas las configuraciones automáticamente
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TurnosDbContext).Assembly);
    }
}
