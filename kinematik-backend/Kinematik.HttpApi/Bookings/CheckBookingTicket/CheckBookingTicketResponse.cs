namespace Kinematik.HttpApi.Bookings.CheckBookingTicket
{
    public class CheckBookingTicketResponse
    {
        public int? ErrorCode { get; set; }
        public bool IsValid => this.ErrorCode == null;
    }
}