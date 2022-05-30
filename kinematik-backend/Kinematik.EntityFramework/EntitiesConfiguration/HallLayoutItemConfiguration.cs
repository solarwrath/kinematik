using Kinematik.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kinematik.EntityFramework.EntitiesConfiguration
{
    public class HallLayoutItemConfiguration : IEntityTypeConfiguration<HallLayoutItem>
    {
        public void Configure(EntityTypeBuilder<HallLayoutItem> builder)
        {
            builder.HasKey(hallLayoutItem => new
            {
                hallLayoutItem.HallID,
                hallLayoutItem.RowID,
                hallLayoutItem.ColumnID
            });
        }
    }
}
