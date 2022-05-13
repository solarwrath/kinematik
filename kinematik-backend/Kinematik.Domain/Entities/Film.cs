namespace Kinematik.Domain.Entities
{
    public class Film
    {
        public int ID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Rating { get; set; }
        public int Duration { get; set; }

        public virtual ICollection<FilmGenre> Genres { get; set; }
    }
}