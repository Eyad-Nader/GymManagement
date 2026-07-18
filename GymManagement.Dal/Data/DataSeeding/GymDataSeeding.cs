using GymManagement.Dal.Data;
using GymManagement.DAL.Data.DbContexts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using GymManagement.DAL.Data.Models;
using Microsoft.Extensions.Logging;
namespace GymManagement.Dal.Data.DataSeeding;
public static class GymDataSeeding
{

    public static async Task SeedPlans(GymDbContext context, string FolderName, ILogger logger, CancellationToken ct = default)
    {
        try
        {
            if (!await context.Plans.AnyAsync(ct))
            {
                var plans = LoadDataFromJsonFile<Plan>(FolderName, "Plans.json");
                if (plans.Any())
                {
                    context.Plans.AddRange(plans);
                    logger.LogInformation($"Retrived {plans.Count} plans");
                }
                if (context.ChangeTracker.HasChanges())
                {
                    var result = await context.SaveChangesAsync(ct);
                    logger.LogInformation($"Added {result} plans");
                }
                else
                {
                    logger.LogInformation("Plans Already Seeded");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error Seeding Plans");
            throw;
        }
    }

    private static List<T> LoadDataFromJsonFile<T>(string FolderName, string fileName)
    {
        var PathFile = Path.Combine(FolderName, fileName);
        if (!File.Exists(PathFile))
            throw new FileNotFoundException($"File not found: {PathFile}");

        var Data = File.ReadAllText(PathFile);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        return JsonSerializer.Deserialize<List<T>>(Data, options) ?? [];
    }

}