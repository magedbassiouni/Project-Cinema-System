
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project_Cinema_System.Data;
using Project_Cinema_System.Models;
using Project_Cinema_System.Repositories;
using Project_Cinema_System.Services;

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
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // 4. Register Generic Repository
            builder.Services.AddScoped(
                typeof(IGenericRepository<>),
                typeof(GenericRepository<>));

            // 5. Register Email Service
            builder.Services.AddScoped<IEmailService, EmailService>();
             
            // 6. Register Promotion Service
            builder.Services.AddScoped<PromotionService>();
            builder.Services.AddScoped<CartService>();
            var app = builder.Build();

            // 7. Initialize Database (Roles + Super Admin)
            await DbInitializer.InitializeAsync(
                app.Services,
                app.Configuration);

            // 8. Configure HTTP Request Pipeline
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

            // 9. Area Route
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

            // 10. Default Route
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
