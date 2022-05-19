using Kinematik.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kinematik.EntityFramework.EntitiesConfiguration
{
    public class HallConfiguration : IEntityTypeConfiguration<Hall>
    {
        public void Configure(EntityTypeBuilder<Hall> builder)
        {
            builder.Property(hall => hall.Title)
                .IsRequired()
                .IsUnicode();
            builder.HasIndex(hall => hall.Title).IsUnique();
        }
    }
}