using System;
using System.Text.Json;
using Core.Models;

namespace Infrastructure.Data;

public class MediCallContextSeed
{
    public static async Task SeedDataAsync(MediCallContext context)
    {
        if (!context.Users.Any())
        {
            var seedData = await File.ReadAllTextAsync("../Data/SeedData/seed-data.Json");
            if (!File.Exists(seedData))
            {
                Console.WriteLine("Seed data file not found");
                return;
            }
            var data = JsonSerializer.Deserialize<SeedData>(seedData, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (data != null)
            {
                Console.WriteLine("Seeding Users...");
                context.Users.AddRange(data.AppUsers);
                Console.WriteLine("Done");

                context.Certificates.AddRange(data.Certificates);

                // Seed NurseCertificates
                context.NurseCertificates.AddRange(data.NurseCertificates);

                // Seed Illnesses
                context.Illnesses.AddRange(data.Illnesses);

                // Seed PatientIllnesses
                context.PatientIllnesses.AddRange(data.PatientIllnesses);

                // Seed Services
                context.Services.AddRange(data.Services);

                // Seed Visits
                context.Visits.AddRange(data.Visits);

                // Seed Payments
                context.Payments.AddRange(data.Payments);

                // Seed Reviews
                context.Reviewings.AddRange(data.Reviews);

                // Seed Notifications
                context.Notifications.AddRange(data.Notifications);

                // Seed ChatReferences
                context.ChatReferences.AddRange(data.ChatReferences);

                await context.SaveChangesAsync();
            }
        }
    }

}
public class SeedData
{
    public List<AppUser> AppUsers { get; set; }
    public List<Certificate> Certificates { get; set; }
    public List<NurseCertificate> NurseCertificates { get; set; }
    public List<Illness> Illnesses { get; set; }
    public List<PatientIllnesses> PatientIllnesses { get; set; }
    public List<Service> Services { get; set; }
    public List<Visit> Visits { get; set; }
    public List<Payment> Payments { get; set; }
    public List<Reviewing> Reviews { get; set; }
    public List<Notification> Notifications { get; set; }
    public List<ChatReference> ChatReferences { get; set; }

}
