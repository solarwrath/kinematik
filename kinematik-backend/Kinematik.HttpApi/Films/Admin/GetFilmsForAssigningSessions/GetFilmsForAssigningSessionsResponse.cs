namespace Kinematik.HttpApi.Films.Admin.GetFilmsForAssigningSessions
{
    public class GetFilmsForAssigningSessionsResponse
    {
        public IEnumerable<GetFilmsForAssigningSessionsResponseMappedFilm> Films { get; set; }
    }

    public class GetFilmsForAssigningSessionsResponseMappedFilm
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int Runtime { get; set; }
    }
}