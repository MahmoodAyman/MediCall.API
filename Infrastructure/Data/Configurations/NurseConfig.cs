using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class NurseConfig : IEntityTypeConfiguration<Nurse>
    {
        public void Configure(EntityTypeBuilder<Nurse> builder)
        {
            builder.ToTable("Nurses");

            builder.Property(n => n.LicenseNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(n => n.ExperienceYears)
                .IsRequired()
                .HasDefaultValue(0);
            
            builder.Property(n => n.IsAvailable)
                .IsRequired()
                .HasDefaultValue(false);
            
            builder.Property(n => n.VisitCount)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(n => n.IsVerified)
                .IsRequired()
                .HasDefaultValue(false);
            
            builder.HasMany(n => n.Visits)
                .WithOne(v => v.Nurse)
                .HasForeignKey(v => v.NurseId);
            
            builder.HasMany(n => n.Certificates)
                .WithOne(c => c.Nurse)
                .HasForeignKey(c => c.NurseId);

            builder.HasMany(n => n.Services)
                .WithMany(s => s.Nurses);

        }
    }
}
