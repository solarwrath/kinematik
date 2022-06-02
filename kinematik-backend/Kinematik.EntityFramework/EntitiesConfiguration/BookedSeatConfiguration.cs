using Kinematik.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kinematik.EntityFramework.EntitiesConfiguration
{
    public class BookedSeatConfiguration : IEntityTypeConfiguration<BookedSeat>
    {
        public void Configure(EntityTypeBuilder<BookedSeat> builder)
        {
            builder.HasKey(bookedSeat => new
            {
                bookedSeat.SessionID,
                bookedSeat.RowID,
                bookedSeat.ColumnID
            });

            builder.HasOne(bookedSeat => bookedSeat.Booking)
                .WithMany(booking => booking.BookedSeats)
                .HasForeignKey(bookedSeat => bookedSeat.BookingID);
        }
    }
}