namespace Kinematik.Domain.Entities
{
    public class Booking
    {
        public int ID { get; set; }
        public int SessionID { get; set; }
        public Session Session { get; set; }
        public string ClientPhone { get; set; }
        public string ClientEmail { get; set; }
        public DateTime BookedAt { get; set; }

        public virtual ICollection<BookedSeat> BookedSeats { get; set; }
    }
}