using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TurnosApp.Domain.Entities;

namespace TurnosApp.Infrastructure.Data.Configurations;

public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.HasOne(s => s.Professional)
            .WithMany(p => p.Schedules)
            .HasForeignKey(s => s.ProfessionalId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(s => new { s.ProfessionalId, s.DayOfWeek });
        
        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}
