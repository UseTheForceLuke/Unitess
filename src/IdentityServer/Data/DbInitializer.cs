using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityServer.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(
        IServiceProvider services,
        IConfiguration configuration)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
        await context.Database.MigrateAsync();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Create roles
        foreach (var role in new[] { Roles.Admin, Roles.User })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Create default admin
        var adminEmail = configuration["SeedData:AdminEmail"];
        var adminPassword = configuration["SeedData:AdminPassword"];

        if (!string.IsNullOrWhiteSpace(adminEmail) && await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new IdentityUser
            {
                UserName = "admin",
                Email = adminEmail
            };

            await userManager.CreateAsync(admin, adminPassword);
            await userManager.AddToRoleAsync(admin, Roles.Admin);
        }
    }

    /// <summary>
    /// For testing purposes only, not for production
    /// </summary>
    public static async Task InitializeTestUsersAsync(
        IServiceProvider services,
        IConfiguration configuration)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var testUsers = new List<(string Email, string Username, string Password, string Role)>
        {
            ("manager@test.com", "manager1", "ManagerPass123!", Roles.Admin),
            ("user1@test.com", "user1", "UserPass123!", Roles.User),
            ("user2@test.com", "user2", "UserPass456!", Roles.User),
            ("user3@test.com", "user3", "UserPass101112!", Roles.User),
            ("audit@test.com", "auditor", "AuditPass789!", Roles.Admin)
        };

        foreach (var userData in testUsers)
        {
            if (await userManager.FindByEmailAsync(userData.Email) == null)
            {
                var user = new IdentityUser
                {
                    UserName = userData.Username,
                    Email = userData.Email,
                    EmailConfirmed = true // Skip verification for test users
                };

                var result = await userManager.CreateAsync(user, userData.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, userData.Role);
                    Console.WriteLine($"Created user: {userData.Username}");
                }
                else
                {
                    Console.WriteLine($"Failed to create {userData.Username}: {string.Join(", ", result.Errors)}");
                }
            }
        }
    }
}