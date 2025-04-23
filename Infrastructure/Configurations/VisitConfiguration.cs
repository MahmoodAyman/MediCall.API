using System;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class VisitConfiguration : IEntityTypeConfiguration<Visit>
{
    public void Configure(EntityTypeBuilder<Visit> builder)
    {
        // Specify the table
        builder.ToTable("Visits");

        // Primary Key
        builder.HasKey(v => v.Id);

        // Required Dates
        builder.Property(v => v.ScheduledDate).IsRequired();

        builder.Property(v => v.ActualVisitDate).IsRequired();

        // Enum conversion 
        builder.Property(v => v.Status).HasConversion<string>().IsRequired().HasMaxLength(50);

        builder.Property(v => v.TransportationCost).HasColumnType("decimal(10,2)").IsRequired();

        // Ignoring Comuted proprties
        builder.Ignore(v => v.ServiceCost);
        // builder.Ignore(v => v.TotalCost);

    }
}
