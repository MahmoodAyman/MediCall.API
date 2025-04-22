using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class VisitConfig : IEntityTypeConfiguration<Visit>
    {
        public void Configure(EntityTypeBuilder<Visit> builder)
        {
            builder.Property(v => v.ActualVisitDate)
                .HasColumnType("datetime");

            builder.Property(v => v.ScheduledDate)
                .IsRequired()
                .HasColumnType("datetime");

            builder.Property(v => v.Status)
                .IsRequired();

            builder.Property(v => v.Notes)
                .HasMaxLength(500);

            builder.Property(v => v.CancellationReason)
                .HasMaxLength(500);

            builder.Property(v => v.TransportationCost)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.OwnsOne(v => v.PatientLocation, l =>
            {
                l.Property(x => x.Lat)
                    .HasColumnType("decimal(9, 6)")
                    .IsRequired();
                l.Property(x => x.Lng)
                    .HasColumnType("decimal(9, 6)")
                    .IsRequired();
            });

            builder.OwnsOne(v => v.NurseLocation, l =>
            {
                l.Property(x => x.Lat)
                    .HasColumnType("decimal(9, 6)")
                    .IsRequired();
                l.Property(x => x.Lng)
                    .HasColumnType("decimal(9, 6)")
                    .IsRequired();
            });

            builder.Property(v => v.NurseId)
                .IsRequired()
                .HasMaxLength(14)
                .IsFixedLength(true)
                .IsUnicode(false);

            builder.Property(v => v.PatientId)
                .IsRequired()
                .HasMaxLength(14)
                .IsFixedLength(true)
                .IsUnicode(false);

            builder.HasMany(v => v.Services)
                .WithMany(s => s.Visits)
                .UsingEntity(j => j.ToTable("VisitServices"));

            builder.HasOne(v => v.Nurse)
                .WithMany(n => n.Visits)
                .HasForeignKey(v => v.NurseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.Patient)
                .WithMany(p => p.Visits)
                .HasForeignKey(v => v.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.Payment)
                .WithOne(p => p.Visit)
                .HasForeignKey<Payment>(p => p.VisitId);

            builder.HasOne(v => v.Reviewing)
                .WithOne(r => r.Visit)
                .HasForeignKey<Reviewing>(r => r.VisitId);


        }
    }
}
