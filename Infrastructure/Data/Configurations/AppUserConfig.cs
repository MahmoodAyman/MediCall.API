using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class AppUserConfig : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.Property(u => u.Id)
                .HasMaxLength(14)
                .IsFixedLength(true)
                .IsUnicode(false);

            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("CK_AppUser_Id_EgyptianID", "Id LIKE '[2,3]%' AND LEN(Id) = 14");
            });


            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.DateOfBirth)
                .HasColumnType("date");

            builder.Property(u => u.Gender)
                .IsRequired();

            builder.Property(u => u.ProfilePicture)
                .HasMaxLength(200);

            builder.OwnsOne(u => u.Location, l =>
            {
                l.Property(x => x.Lat)
                    .HasColumnType("decimal(9, 6)")
                    .IsRequired();
                l.Property(x => x.Lng)
                    .HasColumnType("decimal(9, 6)")
                    .IsRequired();
            });

            builder.OwnsMany(u => u.RefreshTokens, rt =>
            {
                rt.WithOwner();

                rt.Property(r => r.Token)
                  .IsRequired()
                  .HasMaxLength(512);

                rt.Property(r => r.ExpiresOn)
                .IsRequired();

                rt.Property(r => r.CreateOn)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            });

            builder.Property(u => u.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(u => u.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(u => u.UpdatedAt)
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnUpdate();

            builder.HasMany(u => u.Notifications)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.Id);

            builder.HasMany(u => u.ChatReferences)
                .WithOne();

        }
    }
}
