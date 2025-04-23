using System;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class NurseConfiguration : IEntityTypeConfiguration<Nurse>
{
    public void Configure(EntityTypeBuilder<Nurse> builder)
    {
        builder.Property(n => n.LicenseNumber).IsRequired().HasMaxLength(9);
        builder.Property(n => n.IsAvailable).HasDefaultValue(false);
        builder.Property(n => n.IsVerified).HasDefaultValue(false);
        
    }
}
