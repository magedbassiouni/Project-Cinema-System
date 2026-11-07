
using Microsoft.AspNetCore.Identity;
using Project_Cinema_System.Models;

namespace Project_Cinema_System.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            using var scope = serviceProvider.CreateScope();

            var roleManager = scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();

            // 1. Seed Roles
            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(
                        new IdentityRole(role));

                    if (!result.Succeeded)
                    {
                        throw new Exception(
                            $"Failed to create role {role}: " +
                            string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }

            // 2. Get Super Admin settings
            var adminEmail = configuration["SuperAdmin:Email"];
            var adminPassword = configuration["SuperAdmin:Password"];
            var adminName = configuration["SuperAdmin:FullName"];

            if (string.IsNullOrWhiteSpace(adminEmail) ||
                string.IsNullOrWhiteSpace(adminPassword) ||
                string.IsNullOrWhiteSpace(adminName))
            {
                throw new Exception("SuperAdmin settings are missing.");
            }

            // 3. Check if Super Admin exists
            var superAdmin = await userManager.FindByEmailAsync(adminEmail);

            // 4. Create Super Admin if not exists
            if (superAdmin == null)
            {
                superAdmin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = adminName,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(
                    superAdmin, adminPassword);

                if (!createResult.Succeeded)
                {
                    throw new Exception(
                        "Failed to create Super Admin: " +
                        string.Join(", ", createResult.Errors.Select(e => e.Description)));
                }
            }

            // 5. Assign Admin Role
            if (!await userManager.IsInRoleAsync(superAdmin, "Admin"))
            {
                var roleResult = await userManager.AddToRoleAsync(
                    superAdmin, "Admin");

                if (!roleResult.Succeeded)
                {
                    throw new Exception(
                        "Failed to assign Admin role: " +
                        string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
