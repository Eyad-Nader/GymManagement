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
    internal class GymUserConfiguration<T> : IEntityTypeConfiguration<T> where T : GymUser
    {
        public  virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(gu => gu.Id);
            builder.Property(gu => gu.Name)
                .HasColumnType("nvarchar")
                .HasMaxLength(50);
            builder.Property(gu => gu.Email)
                .HasColumnType("nvarchar")
                .HasMaxLength(100);

            builder.HasIndex(gu => gu.Email).IsUnique();
            builder.HasIndex(gu => gu.Phone).IsUnique();

            builder.OwnsOne(gu => gu.Address, address =>
            {
                address.Property(a => a.buildingNumber)
                    .HasColumnType("nvarchar")
                    .HasMaxLength(30);
                address.Property(a => a.City)
                    .HasColumnType("nvarchar")
                    .HasMaxLength(30);
                address.Property(a => a.Street)
                    .HasColumnType("nvarchar")
                    .HasMaxLength(30);
            });

        }
    }
}
