using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class NotificationConfig : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(100);
            
            builder.Property(n => n.Body)
                .HasMaxLength(500);
            
            builder.Property(n => n.Type)
                .IsRequired();
            
            builder.Property(n => n.IsRead)
                .IsRequired()
                .HasDefaultValue(false);
            
            builder.Property(n => n.UserId)
                .IsRequired()
                .HasMaxLength(14)
                .IsFixedLength(true)
                .IsUnicode(false);
            
            builder.HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId);
        }
    }
}
