using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class PaymentConfig : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.Property(p => p.PaymentMethod)
                .IsRequired();
           
            builder.Property(p => p.PaymentDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()");
            
            builder.Property(p => p.Status)
                .IsRequired();
            
            builder.Property(p => p.TransactionReference)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.VisitId)
                .IsRequired();

            builder.HasOne(p => p.Visit)
                .WithOne(v => v.Payment)
                .HasForeignKey<Payment>(p => p.VisitId);
        }
    }
}
