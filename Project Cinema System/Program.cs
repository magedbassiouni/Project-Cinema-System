
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Project_Cinema_System.Data;
using Project_Cinema_System.Models;
using Project_Cinema_System.Repositories;
using System;
using System.Threading.Tasks;

namespace Project_Cinema_System
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Register MVC
            builder.Services.AddControllersWithViews();

            // 2. Register Database
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString(
                        "DefaultConnection")));

            // 3. Register Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // 4. Register Generic Repository
            builder.Services.AddScoped(
                typeof(IGenericRepository<>),
                typeof(GenericRepository<>));

            var app = builder.Build();

            // 5. Create Roles if they do not exist
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider
                    .GetRequiredService<RoleManager<IdentityRole>>();

                string[] roles = { "Admin", "User" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        var result = await roleManager.CreateAsync(
                            new IdentityRole(role));

                        if (!result.Succeeded)
                        {
                            throw new InvalidOperationException(
                                $"Failed to create role: {role}");
                        }
                    }
                }
            }

            // 6. Configure HTTP Request Pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
