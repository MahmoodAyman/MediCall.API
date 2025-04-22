using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class ServiceConfig : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(s => s.BasePrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.HasMany(s => s.Visits)
                .WithMany(v => v.Services)
                .UsingEntity(j => j.ToTable("VisitServices"));

            builder.HasMany(s => s.Nurses)
                .WithMany(n => n.Services)
                .UsingEntity(j => j.ToTable("NurseServices"));

        }
    }
}
