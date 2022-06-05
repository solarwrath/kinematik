using Kinematik.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kinematik.EntityFramework.EntitiesConfiguration
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasOne(booking => booking.Session)
                .WithMany(session => session.Bookings)
                .HasForeignKey(booking => booking.SessionID);
        }
    }
}