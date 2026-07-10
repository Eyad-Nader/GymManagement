using GymManagement.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GymManagement.DAL.Data.DbContexts
{
    public class GymDbContext : DbContext
    {

        public GymDbContext(DbContextOptions<GymDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            modelBuilder.Entity<Plan>().HasData(
            new Plan
            {
                Id = 1,
                Name = "Basic Plan",
                Description = "Access to gym only",
                Duration = 30,
                Price = 300,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1)
            },
            new Plan
            {
                Id = 2,
                Name = "Standard Plan",
                Description = "Gym + Classes",
                Duration = 60,
                Price = 500,
                IsActive = false,
                CreatedAt = new DateTime(2026, 1, 1)
            },
            new Plan
            {
                Id = 3,
                Name = "Premium Plan",
                Description = "All access + Personal Trainer",
                Duration = 90,
                Price = 900,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1)
            },
            new Plan
            {
                Id = 4,
                Name = "3000",
                Description = "Full Year Plan",
                Duration = 356,
                Price = 1500,
                IsActive = false,
                CreatedAt = new DateTime(2026, 1, 1)
            }
            );
        }

        public DbSet<Plan> Plans { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<MemberShip> MemberShips { get; set; }
        public DbSet<HealthRecord> HealthRecords { get; set; }
        public DbSet<Booking> Bookings { get; set; }


    }
}
