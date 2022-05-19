namespace Kinematik.Domain.Entities
{
    public class Hall
    {
        public int ID { get; set; }
        public string Title { get; set; }

        public virtual ICollection<Session> Sessions { get; set; } = null!;
    }
}
