using Microsoft.AspNetCore.Http;

namespace Kinematik.HttpApi.Films.Admin.CreateFilm
{
    public class CreateFilmRequest
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? SerializedGenreIDs { get; set; }
        public string? SerializedRuntime { get; set; }
        public string? ImdbID { get; set; }
        public string? TrailerUrl { get; set; }

        public IFormFile? Poster { get; set; }
        public IFormFile? FeaturedImage { get; set; }
    }   
}