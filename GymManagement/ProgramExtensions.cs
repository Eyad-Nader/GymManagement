using GymManagement.Dal.Data.DataSeeding;
using GymManagement.DAL.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Data.DataSeeding;

namespace GymManagement
{
    public static class ProgramExtensions
    {   
        public static async Task MigrationsAndSeedFilesAsync(this WebApplication app)
        
        {
            using var Scope = app.Services.CreateScope();
            var DbContext = Scope.ServiceProvider.GetRequiredService<GymDbContext>();
            var logger = Scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var roleManager = Scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = Scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var PendingMigrations = DbContext.Database.GetPendingMigrations();
            if(PendingMigrations.Any())
            {
                logger.LogInformation($"Applying {PendingMigrations.Count()} Pending Migrations");
                await DbContext.Database.MigrateAsync();
                logger.LogInformation("Migration Applied Successfully");
            }
            var FolderPath = Path.Combine(app.Environment.ContentRootPath,"wwwroot","Files");
            if(!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
                logger.LogInformation("Files Directory Created Successfully");
            }
            await GymDataSeeding.SeedPlans(DbContext,FolderPath ,logger);
            await IdentityDataSeeding.SeedIdentityDataAsync(roleManager, userManager, logger);
        }
    

    }
}   