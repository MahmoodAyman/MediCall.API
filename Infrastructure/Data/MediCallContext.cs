using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Core.Models;
using Infrastructure.Data.Configurations;
using Microsoft;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class MediCallContext(DbContextOptions options) : IdentityDbContext<AppUser>(options)
    {
        // TODO: Create DbSets
        public DbSet<Nurse> Nurses { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<NurseCertificate> NurseCertificates { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ChatReference> ChatReferences { get; set; }
        public DbSet<Illness> Illnesses { get; set; }
        public DbSet<PatientIllnesses> PatientIllnesses { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Reviewing> Reviewings { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType).Property("IsDeleted").HasDefaultValue(false);
                    modelBuilder.Entity(entityType.ClrType).Property("CreatedAt").HasDefaultValueSql("GETDATE()");
                    modelBuilder.Entity(entityType.ClrType).Property("UpdatedAt").HasDefaultValueSql("GETDATE()").ValueGeneratedOnUpdate();
                }
            }


            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new AppUserConfig());

            modelBuilder.ApplyConfiguration(new CertificateConfig());

            modelBuilder.ApplyConfiguration(new ChatReferenceConfig());

            modelBuilder.ApplyConfiguration(new IllnessConfig());
            
            modelBuilder.ApplyConfiguration(new NotificationConfig());
            
            modelBuilder.ApplyConfiguration(new NurseCertificateConfig());
            
            modelBuilder.ApplyConfiguration(new NurseConfig());
            
            modelBuilder.ApplyConfiguration(new PatientConfig());
            
            modelBuilder.ApplyConfiguration(new PatientIllnessesConfig());
            
            modelBuilder.ApplyConfiguration(new PaymentConfig());
            
            modelBuilder.ApplyConfiguration(new ReviewingConfig());
            
            modelBuilder.ApplyConfiguration(new ServiceConfig());
            
            modelBuilder.ApplyConfiguration(new VisitConfig());
        }
    }
}
