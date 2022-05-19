namespace Kinematik.Domain.Entities
{
    public class Session
    {
        public int ID { get; set; }
        public int FilmID { get; set; }
        public int? HallID { get; set; }
        public DateTime StartAt { get; set; }

        public Film Film { get; set; }
        public Hall? Hall { get; set; }
    }
}