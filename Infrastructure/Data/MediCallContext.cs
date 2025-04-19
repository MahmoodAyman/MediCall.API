using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Core.Models;
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
            base.OnModelCreating(modelBuilder);

            // Owned Types
            modelBuilder.Entity<AppUser>()
                .OwnsOne(u => u.Location);

            modelBuilder.Entity<Visit>()
                .OwnsOne(v => v.PatientLocation);

            modelBuilder.Entity<Visit>()
                .OwnsOne(v => v.NurseLocation);

            // Visit
            modelBuilder.Entity<Visit>()
                .HasOne(v => v.Nurse)
                .WithMany(n => n.Visits)
                .HasForeignKey(v => v.NurseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Visit>()
                .HasOne(v => v.Patient)
                .WithMany(p => p.Visits)
                .HasForeignKey(v => v.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // NurseCertificate
            modelBuilder.Entity<NurseCertificate>()
                .HasKey(nc => new { nc.NurseId, nc.CertificateId });

            modelBuilder.Entity<NurseCertificate>()
                .HasOne(nc => nc.Nurse)
                .WithMany(n => n.Certificates)
                .HasForeignKey(nc => nc.NurseId);

            modelBuilder.Entity<NurseCertificate>()
                .HasOne(nc => nc.Certificate)
                .WithMany()
                .HasForeignKey(nc => nc.CertificateId);

            // Notification
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId);

            // ChatReference
            modelBuilder.Entity<ChatReference>()
                .HasOne(c => c.Patient)
                .WithMany()
                .HasForeignKey(c => c.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatReference>()
                .HasOne(c => c.Nurse)
                .WithMany()
                .HasForeignKey(c => c.NurseId)
                .OnDelete(DeleteBehavior.Restrict);

            // PatientIllnesses
            modelBuilder.Entity<PatientIllnesses>()
                .HasKey(pi => new { pi.PatientId, pi.IllnessId });

            modelBuilder.Entity<PatientIllnesses>()
                .HasOne(pi => pi.Patient)
                .WithMany(p => p.PatientIllnesses)
                .HasForeignKey(pi => pi.PatientId);

            modelBuilder.Entity<PatientIllnesses>()
                .HasOne(pi => pi.Illness)
                .WithMany()
                .HasForeignKey(pi => pi.IllnessId);

            // Payment
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Visit)
                .WithOne()
                .HasForeignKey<Payment>(p => p.VisitId);

            // Reviewing
            modelBuilder.Entity<Reviewing>()
                .Property(r => r.Rating)
                .HasDefaultValue(0)
                .IsRequired();


            modelBuilder.Entity<Reviewing>()
                .HasOne(r => r.Visit)
                .WithOne()
                .HasForeignKey<Reviewing>(r => r.VisitId);

            // Visit - Services
            modelBuilder.Entity<Visit>()
                .HasMany(v => v.Services)
                .WithMany();

            // TODO: Add other configurations as needed ,Add Validation And remove the TODO comment
        }
    }
}
