namespace Kinematik.Domain.Entities
{
    public class Film
    {
        public int ID { get; set; }
        public string Title { get; set; } = null!;
        public string? PosterPath { get; set; }
        public string Description { get; set; } =  null!;
        public int? LanguageID { get; set; }
        public string? ImdbID { get; set; }
        public int? Runtime { get; set; }
        public string? TrailerUrl { get; set; }
        public string? FeaturedImagePath { get; set; }

        public Language? Language { get; set; }

        public virtual ICollection<FilmToGenrePair> GenrePairs { get; set; } = null!;
        public virtual ICollection<Session> Sessions { get; set; } = null!;
    }
}