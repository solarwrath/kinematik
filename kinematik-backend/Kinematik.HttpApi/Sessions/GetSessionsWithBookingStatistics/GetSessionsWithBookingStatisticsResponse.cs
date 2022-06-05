namespace Kinematik.HttpApi.Sessions.GetSessionsWithBookingStatistics
{
    public class GetSessionsWithBookingStatisticsResponse
    {
        public IEnumerable<GetSessionsWithBookingStatisticsResponseSession> Sessions { get; set; }
    }

    public class GetSessionsWithBookingStatisticsResponseSession
    {
        public int ID { get; set; }
        public int FilmID { get; set; }
        public string FilmTitle { get; set; }
        public string? PosterUrl { get; set; }
        public int HallID { get; set; }
        public string HallTitle { get; set; }
        public int HallCapacity { get; set; }
        public int BookedSeatsQuantity { get; set; }
        public DateTime StartAt { get; set; }
    }
}