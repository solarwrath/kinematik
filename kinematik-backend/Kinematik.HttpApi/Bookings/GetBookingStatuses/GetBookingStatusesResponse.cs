namespace Kinematik.HttpApi.Bookings.GetBookingStatuses
{
    public class GetBookingStatusesResponse
    {
        public IEnumerable<GetBookingStatusesResponseBookingStatus> BookingStatuses { get; set; }
    }

    public class GetBookingStatusesResponseBookingStatus
    {
        public int RowID { get; set; }
        public int ColumnID { get; set; }
        public int SeatTypeID { get; set; }
        public bool IsFree { get; set; }
    }
}