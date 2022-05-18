namespace Kinematik.HttpApi.Films.Admin.GetFilmsList
{
    public class GetFilmsListResponse
    {
        public IEnumerable<GetFilmsListResponseMappedFilm> Films { get; set; }

    }
    public class GetFilmsListResponseMappedFilm
    {
        public int ID { get; set; }
        public string Title { get; set; } = null!;
        public string? PosterUrl { get; set; }
        public string Description { get; set; } = null!;
        public IEnumerable<int>? GenreIDs { get; set; }
    }
}