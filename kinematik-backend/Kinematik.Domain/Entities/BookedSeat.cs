namespace Kinematik.Domain.Entities
{
    public class BookedSeat
    {
        public int BookingID { get; set; }
        public Booking Booking { get; set; }
        public int RowID { get; set; }
        public int ColumnID { get; set; }
        public int SessionID { get; set; }
    }
}