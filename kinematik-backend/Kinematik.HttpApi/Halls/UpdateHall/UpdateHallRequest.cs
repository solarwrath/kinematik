namespace Kinematik.HttpApi.Halls.UpdateHall
{
    public class UpdateHallRequest
    {
        public string Title { get; set; }
        public IEnumerable<UpdateHallRequestLayoutItem> LayoutItems { get; set; }
    }

    public class UpdateHallRequestLayoutItem
    {
        public int RowID { get; set; }
        public int ColumnID { get; set; }
        public int SeatTypeID { get; set; }
    }
}