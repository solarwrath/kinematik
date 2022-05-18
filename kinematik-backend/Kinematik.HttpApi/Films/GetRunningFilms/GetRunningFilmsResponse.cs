namespace Kinematik.HttpApi.Films.GetRunningFilms
{
    public class GetRunningFilmsResponse
    {
        public IEnumerable<GetRunningFilmsResponseFilm> RunningFilms { get; set; }
    }

    public class GetRunningFilmsResponseFilm
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string PosterUrl { get; set; }
    }
}