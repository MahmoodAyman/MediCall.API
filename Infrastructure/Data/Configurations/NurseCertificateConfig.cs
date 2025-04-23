using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class NurseCertificateConfig : IEntityTypeConfiguration<NurseCertificate>
    {
        public void Configure(EntityTypeBuilder<NurseCertificate> builder)
        {
            builder.HasKey(nc => new { nc.NurseId, nc.CertificateId });

            builder.Property(nc => nc.NurseId)
                .IsRequired()
                .HasMaxLength(14)
                .IsFixedLength(true)
                .IsUnicode(false);

            builder.Property(nc => nc.CertificateId)
                .IsRequired();

            builder.Property(nc => nc.FilePath)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(nc => nc.ExpirationDate)
                .HasColumnType("date");

            builder.Property(nc => nc.IsVerified)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasOne(nc => nc.Nurse)
                .WithMany(n => n.Certificates)
                .HasForeignKey(nc => nc.NurseId);

            builder.HasOne(nc => nc.Certificate)
                .WithMany()
                .HasForeignKey(nc => nc.CertificateId);
        }
    }
}
