using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TurnosApp.Domain.Entities;

namespace TurnosApp.Infrastructure.Data.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.DocumentNumber)
            .HasMaxLength(20);
        
        builder.Property(p => p.Address)
            .HasMaxLength(300);
        
        builder.Property(p => p.MedicalInsurance)
            .HasMaxLength(100);
        
        builder.Property(p => p.InsuranceNumber)
            .HasMaxLength(50);
        
        builder.HasOne(p => p.User)
            .WithOne(u => u.Patient)
            .HasForeignKey<Patient>(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
