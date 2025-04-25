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
            var AppUsersData = await File.ReadAllTextAsync("../Data/SeedData/AppUsers.Json");
            var AppUsers = JsonSerializer.Deserialize<List<AppUser>>(AppUsersData);
            if (AppUsers == null) return;
            context.Users.AddRange(AppUsers);
            await context.SaveChangesAsync();
        }
    }

}
