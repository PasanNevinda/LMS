using System.Linq.Expressions;
using LMS.Data;
using LMS.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace LMS.Services
{
    public class SeedService
    {
        public static async Task SeedDbAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedService>>();

            try
            {
                // ensure database is created
                logger.LogInformation("Ensuring database is created...");
                await context.Database.EnsureCreatedAsync();

                // add roles
                string[] roles = { "Admin", "Teacher", "Student" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        logger.LogInformation($"Creating role: {role}");
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                // seed admin user
                var adminEmail = "mainadmin@xyz.com";
                var adminPassword = "Admin123@";

                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    logger.LogInformation($"Creating admin user: {adminEmail}");
                    adminUser = new Admin
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FullName = "Main Admin",
                        RegistrationTime = DateTime.UtcNow,
                        IsMainAdmin = true,
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(adminUser, adminPassword);
                    if (result.Succeeded)
                    {
                        logger.LogInformation("Admin user created successfully.");
                        await userManager.AddToRoleAsync(adminUser, "Admin");

                        await context.SaveChangesAsync();
                        logger.LogInformation("Main Admin user created and added to admins table.");
                    }
                    else
                    {
                        logger.LogError("Failed to create admin user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    logger.LogInformation("Admin user already exists.");
                    adminUser.EmailConfirmed = true;
                    await userManager.UpdateAsync(adminUser);
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }
    }
}
