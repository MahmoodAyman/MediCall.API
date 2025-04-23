using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class PatientIllnessesConfig : IEntityTypeConfiguration<PatientIllnesses>
    {
        public void Configure(EntityTypeBuilder<PatientIllnesses> builder)
        {
            builder.HasKey(pi => new { pi.PatientId, pi.IllnessId });
            
            builder.Property(pi => pi.PatientId)
                .IsRequired()
                .HasMaxLength(14)
                .IsFixedLength(true)
                .IsUnicode(false);
            
            builder.Property(pi => pi.IllnessId)
                .IsRequired();
            
            builder.Property(pi => pi.DiagnosisDate)
                .HasColumnType("date");

            builder.Property(pi => pi.Notes)
                .HasMaxLength(500);

            builder.HasOne(pi => pi.Patient)
                .WithMany(p => p.PatientIllnesses)
                .HasForeignKey(pi => pi.PatientId);

            builder.HasOne(pi => pi.Illness)
                .WithMany()
                .HasForeignKey(pi => pi.IllnessId);

        }
    }
}
