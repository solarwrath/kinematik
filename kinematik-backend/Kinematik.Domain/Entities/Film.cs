namespace Kinematik.Domain.Entities
{
    public class Film
    {
        public int ID { get; set; }
        public string Title { get; set; } = null!;
        public string? PosterPath { get; set; }
        public string Description { get; set; } =  null!;
        public string? ImdbID { get; set; }
        public int? Runtime { get; set; }
        public string? TrailerUrl { get; set; }
        public string? FeaturedImagePath { get; set; }
        
        public virtual ICollection<FilmToGenrePair> GenrePairs { get; set; } = null!;
    }
}