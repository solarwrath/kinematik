namespace Kinematik.HttpApi.Bookings.CreateBooking
{
    public class CreateBookingRequest
    {
        public int SessionID { get; set; }
        public IEnumerable<CreateBookingRequestSeatCoordinates> SeatsCoordinates { get; set; }
        public string ClientEmail { get; set; }
        public string ClientPhone { get; set; }
    }

    public class CreateBookingRequestSeatCoordinates
    {
        public int RowID { get; set; }
        public int ColumnID { get; set; }
    }
}