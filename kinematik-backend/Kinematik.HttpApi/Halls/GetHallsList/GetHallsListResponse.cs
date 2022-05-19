namespace Kinematik.HttpApi.Halls.GetHallsList
{
    public class GetHallsListResponse
    {
        public IEnumerable<GetHallsListResponseMappedHall> Halls { get; set; }

    }
    public class GetHallsListResponseMappedHall
    {
        public int ID { get; set; }
        public string Title { get; set; } = null!;
    }
}