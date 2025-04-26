using System;
using System.Text.Json;
using Core.Enums;
using Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data;

public class MediCallContextSeed
{
    // private static MediCallContext _context;
    // private static UserManager<AppUser> _userManager;

    // public MediCallContextSeed(MediCallContext context, UserManager<AppUser> userManager)
    // {
    //     _context = context;
    //     _userManager = userManager;
    // }
    public static async Task SeedDataAsync(MediCallContext context, UserManager<AppUser> userManger, RoleManager<IdentityRole> roleManager)
    {
        // if (context.Users.Any())
        // {
        //     var seedData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/seed-data.Json");
        //     // if (!File.Exists(seedData))
        //     // {
        //     //     Console.WriteLine("Seed data file not found");
        //     //     return;
        //     // }
        //     var data = JsonSerializer.Deserialize<SeedData>(seedData, new JsonSerializerOptions
        //     {
        //         PropertyNameCaseInsensitive = true
        //     });
        //     if (data != null)
        //     {
        //         // for (int i = 0; i < data.AppUsers.Count(); i++)
        //         // {
        //         //     if (Enum.TryParse<Gender>(data.AppUsers[i].Gender.ToString(), out var gender))
        //         //     {
        //         //         data.AppUsers[i].Gender = gender;
        //         //     }
        //         // }
        //         // Console.WriteLine("Seeding Users...");

        //         // context.Users.AddRange(data.AppUsers);
        //         // var userRoles = new UserRoles
        //         // context.UserRoles.AddRange();

        //         // foreach (var user in data.AppUsers)
        //         // {
        //         //     var u = await _userManager.FindByIdAsync(user.Id);
        //         //     // _userManager.AddToRoleAsync(u, user)
        //         // }

        //         // Console.WriteLine("Done");

        //         // context.Certificates.AddRange(data.Certificates);

        //         // Seed NurseCertificates
        //         // context.NurseCertificates.AddRange(data.NurseCertificates);

        //         // Seed Illnesses
        //         // context.Illnesses.AddRange(data.Illnesses);

        //         // Seed PatientIllnesses
        //         // context.PatientIllnesses.AddRange(data.PatientIllnesses);

        //         // Seed Services
        //         // context.Services.AddRange(data.Services);

        //         // Seed Visits
        //         // context.Visits.AddRange(data.Visits);

        //         // Seed Payments
        //         // context.Payments.AddRange(data.Payments);

        //         // Seed Reviews
        //         // context.Reviewings.AddRange(data.Reviews);

        //         // Seed Notifications
        //         // context.Notifications.AddRange(data.Notifications);

        //         // Seed ChatReferences
        //         // context.ChatReferences.AddRange(data.ChatReferences);

        //         await context.SaveChangesAsync();
        //     }
        // }

        try
        {
            // await SeedRoleAsync(roleManager);
            var seedData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/seed-data.Json");
            var data = JsonSerializer.Deserialize<SeedData>(seedData, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (data != null)
            {
                Console.WriteLine("Seeding Users...");
                foreach (var user in data.AppUsers)
                {

                    var appUser = new AppUser
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        DateOfBirth = user.DateOfBirth,
                        Gender = user.Gender,
                        Location = user.Location,
                        IsDeleted = user.IsDeleted,
                        CreatedAt = user.CreatedAt,
                        EmailConfirmed = true, // Set to true for development
                        NormalizedEmail = user.Email.ToUpper(),
                        NormalizedUserName = user.UserName.ToUpper()
                    };

                    await userManger.CreateAsync(appUser, "Password");
                    await userManger.AddToRoleAsync(appUser, user.Discriminator);
                    // await context.SaveChangesAsync();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
    private static async Task SeedRoleAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roleNames = { "Admin", "Nurse", "Patinet" };
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));

            }
        }

    }

}
public class SeedData
{
    public List<AppUserSeed> AppUsers { get; set; }
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

public class AppUserSeed
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public Location Location { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Discriminator { get; set; }

    // Nurse specific properties
    public string LicenseNumber { get; set; }
    public int ExperienceYears { get; set; }
    public bool IsAvailable { get; set; }
    public int VisitCount { get; set; }
    public bool IsVerified { get; set; }
}