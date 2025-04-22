using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Models;
namespace Infrastructure.Configurations;

public class NurseCertificateConfiguration : IEntityTypeConfiguration<NurseCertificate>
{

    public void Configure(EntityTypeBuilder<NurseCertificate> builder)
    {
        builder.Property(nc => nc.FilePath).IsRequired().HasMaxLength(255);
        builder.Property(nc => nc.IsVerified).HasDefaultValue(false);
        builder.Ignore(nc => nc.IsExpired);
    }
}
