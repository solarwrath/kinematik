namespace Kinematik.HttpApi.Halls.CreateHall
{
    public class CreateHallRequest
    {
        public string Title { get; set; }
        public IEnumerable<CreateHallRequestLayoutItem> LayoutItems { get; set; }
    }

    public class CreateHallRequestLayoutItem
    {
        public int RowID { get; set; }
        public int ColumnID { get; set; }
        public int TypeID { get; set; }
    }
}