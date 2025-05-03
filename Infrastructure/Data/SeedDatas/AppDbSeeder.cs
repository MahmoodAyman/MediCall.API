using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Enums;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Data.SeedDatas
{
    public class AppDbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Admin
            string adminEmail = "admin@example.com";
            string adminPassword = "Admin@123";

            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                var adminUser = new AppUser
                {
                    Id = "30304012405209",
                    UserName = "admin",
                    Email = adminEmail,
                    FirstName = "Super",
                    LastName = "Admin",
                    PhoneNumber = "01000000000",
                    DateOfBirth = new DateTime(1980, 1, 1),
                    Gender = Gender.Male,
                    Location = new Location(),
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };
                adminUser.EmailConfirmed = true;
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    throw new Exception("Failed to create admin: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }



        }
    }

}
