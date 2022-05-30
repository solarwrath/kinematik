namespace Kinematik.HttpApi.Halls.GetHall
{
    public class GetHallResponse
    {
        public string Title { get; set; }
        public IEnumerable<GetHallResponseLayoutItem> LayoutItems { get; set; }
    }

    public class GetHallResponseLayoutItem
    {
        public int RowID { get; set; }
        public int ColumnID { get; set; }
        public int TypeID { get; set; }
    }
}