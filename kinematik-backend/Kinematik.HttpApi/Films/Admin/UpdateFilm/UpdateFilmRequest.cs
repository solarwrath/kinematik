using Microsoft.AspNetCore.Http;

namespace Kinematik.HttpApi.Films.Admin.UpdateFilm
{
    public class UpdateFilmRequest
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? SerializedGenreIDs { get; set; }
        public string? SerializedRuntime { get; set; }
        public string? ImdbID { get; set; }
        public string? TrailerUrl { get; set; }

        public IFormFile? Poster { get; set; }
        public string SerializedWasPosterDeleted { get; set; }
        public IFormFile? FeaturedImage { get; set; }
        public string SerializedWasFeaturedImageDeleted { get; set; }
    }
}
