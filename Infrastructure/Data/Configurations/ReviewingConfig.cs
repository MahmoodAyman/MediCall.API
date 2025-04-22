using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class ReviewingConfig : IEntityTypeConfiguration<Reviewing>
    {
        public void Configure(EntityTypeBuilder<Reviewing> builder)
        {
            builder.Property(v => v.VisitId)
                .IsRequired();

            builder.Property(r => r.Comment)
                .HasMaxLength(500);

            builder.Property(r => r.Rating)
                .IsRequired()
                .HasDefaultValue(0);

            builder.ToTable(tb => 
            {
                tb.HasCheckConstraint("CK_Reviewing_Rating", "Rating >= 1 AND Rating <= 5");
            });

            builder.HasOne(r => r.Visit)
                .WithOne(v => v.Reviewing)
                .HasForeignKey<Reviewing>(r => r.VisitId);
        }
    }
}
