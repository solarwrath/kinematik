namespace Kinematik.Domain.Entities
{
    public class Genre
    {
        public int ID { get; set; }
        public string Title { get; set; }

        public virtual ICollection<FilmToGenrePair> FilmPairs { get; set; }
    }
}