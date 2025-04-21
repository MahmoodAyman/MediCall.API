using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class CertificateConfig : IEntityTypeConfiguration<Certificate>
    {
        public void Configure(EntityTypeBuilder<Certificate> builder)
        {
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.IsRequired)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.IsExpirable)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.Description)
                .HasMaxLength(500);
        }
    }
}
