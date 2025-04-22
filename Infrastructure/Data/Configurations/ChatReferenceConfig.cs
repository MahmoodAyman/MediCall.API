using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class ChatReferenceConfig : IEntityTypeConfiguration<ChatReference>
    {
        public void Configure(EntityTypeBuilder<ChatReference> builder)
        {
            builder.Property(cr => cr.Id)
                .ValueGeneratedOnAdd();
            
            builder.Property(cr => cr.FirebaseChatId)
                .IsRequired()
                .HasMaxLength(200);
            
            builder.Property(cr => cr.PatientId)
                .IsRequired()
                .HasMaxLength(14)
                .IsFixedLength(true)
                .IsUnicode(false);
            
            builder.Property(cr => cr.NurseId)
                .IsRequired()
                .HasMaxLength(14)
                .IsFixedLength(true)
                .IsUnicode(false);
            
            builder.Property(cr => cr.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
            
            builder.HasOne(cr => cr.Patient)
                .WithMany()
                .HasForeignKey(cr => cr.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cr => cr.Nurse)
                .WithMany()
                .HasForeignKey(cr => cr.NurseId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
