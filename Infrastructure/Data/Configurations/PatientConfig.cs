using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class PatientConfig : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.HasMany(p => p.PatientIllnesses)
                .WithOne(pi => pi.Patient)
                .HasForeignKey(pi => pi.PatientId);

            builder.HasMany(p => p.Visits)
                .WithOne(v => v.Patient)
                .HasForeignKey(v => v.PatientId);
        }
    }
}
