namespace Kinematik.HttpApi.Sessions.GetSessions
{
    public class GetAllSessionsResponse
    {
        public IEnumerable<GetSessionsResponseMappedSession> Sessions { get; set; }

    }
    public class GetSessionsResponseMappedSession
    {
        public int ID { get; set; }
        public int FilmID { get; set; }
        public int? HallID { get; set; }
        public DateTime StartAt { get; set; }
    }
}