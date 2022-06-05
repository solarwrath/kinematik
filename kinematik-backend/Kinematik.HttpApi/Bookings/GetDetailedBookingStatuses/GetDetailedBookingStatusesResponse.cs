namespace Kinematik.HttpApi.Bookings.GetDetailedBookingStatuses
{
    public class GetDetailedBookingStatusesResponse
    {
        public IEnumerable<GetDetailedBookingStatusesResponseBookingStatus> DetailedBookingStatuses { get; set; }
    }

    public class GetDetailedBookingStatusesResponseBookingStatus
    {
        public int RowID { get; set; }
        public int ColumnID { get; set; }
        public int SeatTypeID { get; set; }
        public int SeatAvailabilityStatusID { get; set; }
        public int? BookingOrderID { get; set; }
        public string? BookedClientEmail { get; set; }
        public string? BookedClientPhone { get; set; }
    }
}