using Microsoft.AspNetCore.Identity;
using GymManagement.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace GymManagement.DAL.Data.DataSeeding
{
    public static class IdentityDataSeeding
    {
        public static async Task SeedIdentityDataAsync(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ILogger logger,
            CancellationToken ct = default)
        {
            try
            {
                bool hasUsers = await userManager.Users.AnyAsync(ct);
                bool hasRoles = await roleManager.Roles.AnyAsync(ct);

                if (hasUsers && hasRoles) return;

                var Rules = new List<IdentityRole>
            {
                new IdentityRole { Name = "SuperAdmin" },
                new IdentityRole { Name = "Admin" }
            };
                foreach (var rule in Rules)
                {
                    if (!await roleManager.RoleExistsAsync(rule.Name!))
                    {
                        var RoleResult = await roleManager.CreateAsync(rule);
                        if (!RoleResult.Succeeded)
                        {
                            logger.LogError($"Failed to create rule {rule.Name} : {RoleResult.Errors.Select(x => x.Description)}");
                        }
                    }
                }

                if (!hasUsers)
                {
                    var SuperAdmin = new ApplicationUser
                    {
                        FirstName = "eyad",
                        LastName = "nader",
                        Email = "eyadnader322@gmail.com",
                        UserName = "eyadnader",
                        PhoneNumber = "01150253055",
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                    };
                    await userManager.CreateAsync(SuperAdmin, "SAdmin@123");
                    await userManager.AddToRoleAsync(SuperAdmin, "SuperAdmin");

                    var Admin = new ApplicationUser
                    {
                        FirstName = "Admin",
                        LastName = "Admin",
                        Email = "Admin@gmail.com",
                        UserName = "Admin",
                    };
                    await userManager.CreateAsync(Admin, "Admin@123");
                    await userManager.AddToRoleAsync(Admin, "Admin");

                    logger.LogInformation("Identity Data Seeded Successfully");
                }

                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Seeding Identity Data");
                return;
            }
        }
    }
}