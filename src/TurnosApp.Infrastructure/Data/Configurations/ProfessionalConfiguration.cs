using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TurnosApp.Domain.Entities;

namespace TurnosApp.Infrastructure.Data.Configurations;

public class ProfessionalConfiguration : IEntityTypeConfiguration<Professional>
{
    public void Configure(EntityTypeBuilder<Professional> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.LicenseNumber)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(p => p.LicenseNumber)
            .IsUnique();
        
        builder.HasOne(p => p.User)
            .WithOne(u => u.Professional)
            .HasForeignKey<Professional>(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(p => p.Specialty)
            .WithMany(s => s.Professionals)
            .HasForeignKey(p => p.SpecialtyId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
