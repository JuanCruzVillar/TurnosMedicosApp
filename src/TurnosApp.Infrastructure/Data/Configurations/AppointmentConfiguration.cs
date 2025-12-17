using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TurnosApp.Domain.Entities;

namespace TurnosApp.Infrastructure.Data.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasKey(a => a.Id);
        
        builder.HasOne(a => a.Professional)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.ProfessionalId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Property(a => a.Status)
            .HasConversion<string>();
        
        builder.Property(a => a.Reason)
            .HasMaxLength(500);
        
        builder.Property(a => a.Notes)
            .HasMaxLength(1000);
        
        builder.Property(a => a.CancellationReason)
            .HasMaxLength(500);
        
        builder.HasIndex(a => a.DateTime);
        builder.HasIndex(a => new { a.ProfessionalId, a.DateTime });
        
        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}
