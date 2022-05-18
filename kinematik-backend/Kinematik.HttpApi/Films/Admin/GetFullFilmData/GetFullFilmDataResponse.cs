namespace Kinematik.HttpApi.Films.Admin.GetFullFilmData
{
    public class GetFullFilmDataResponse
    {
        public string Title { get; set; } = null!;
        public string? PosterUrl { get; set; }
        public string Description { get; set; } = null!;
        public IEnumerable<int>? GenreIDs { get; set; }
        public int? Runtime { get; set; }
        public string? ImdbID { get; set; }
        public string? TrailerUrl { get; set; }
        public string? FeaturedImageUrl { get; set; }
    }
}
