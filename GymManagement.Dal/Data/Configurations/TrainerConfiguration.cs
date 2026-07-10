using GymManagement.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Data.Configurations
{
    internal class TrainerConfiguration : GymUserConfiguration<Trainer>
    {
        public override void Configure(EntityTypeBuilder<Trainer> builder)
        {
            builder.Property(t => t.CreatedAt).HasColumnName("HireDate")
                    .HasDefaultValueSql("GETDATE()");
            base.Configure(builder);
        }
    }
}
