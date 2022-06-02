namespace Kinematik.Domain.Entities
{
    public class HallLayoutItem
    {
        public int HallID { get; set; }
        public Hall Hall { get; set; }

        public int RowID { get; set; }
        public int ColumnID { get; set; }
        public SeatType SeatType { get; set; }
    }
}
