namespace Kinematik.HttpApi.Sessions.UpdateAllSessions
{
    public class UpdateAllSessionsRequest
    {
        public IEnumerable<UpdateAllSessionsRequestSession> Sessions { get; set; }
    }

    public class UpdateAllSessionsRequestSession
    {
        public int? ID { get; set; }
        public int FilmID { get; set; }
        public int? HallID { get; set; }
        public DateTime StartAt { get; set; }
    }
}
