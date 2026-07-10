using GymManagement.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagement.DAL.Data.Configurations
{
    public class PlanConfiguration : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.Property(p => p.Name)
                   .IsRequired().HasMaxLength(50);
            builder.Property(p => p.Description)
                   .HasMaxLength(200);
            builder.Property(p => p.Price)
                   .IsRequired()
                   .HasColumnType("decimal(10,2)");
            builder.Property(p => p.IsActive)
                   .IsRequired();
            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("CK_Plan_Duration", "Duration Between 1 and 365");
            });
        }
    }
}
