namespace Kinematik.Domain.Entities
{
    public class FilmToGenrePair
    {
        public int FilmID { get; set; }
        public int GenreID { get; set; }

        public Film Film { get; set; }
        public Genre Genre { get; set; }
    }
}