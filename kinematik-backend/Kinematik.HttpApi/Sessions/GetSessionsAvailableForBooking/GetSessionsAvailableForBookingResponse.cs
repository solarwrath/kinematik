namespace Kinematik.HttpApi.Sessions.GetSessionsAvailableForBooking
{
    public class GetSessionsAvailableForBookingResponse
    {
        public IEnumerable<GetSessionsAvailableForBookingMappedSession> Sessions { get; set; }
    }

    public class GetSessionsAvailableForBookingMappedSession
    {
        public int ID { get; set; }
        public int HallID { get; set; }
        public string HallTitle { get; set; }
        public DateTime StartAt { get; set; }
    }
}