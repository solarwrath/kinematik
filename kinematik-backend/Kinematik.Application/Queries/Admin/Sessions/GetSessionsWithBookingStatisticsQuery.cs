using Kinematik.Application.Ports;
using Kinematik.EntityFramework;

using MediatR;

namespace Kinematik.Application.Queries.Admin.Sessions
{
    public class GetSessionsWithBookingStatisticsQueryInput : IRequest<GetSessionsWithBookingStatisticsQueryOutput>
    {
    }

    public class GetSessionsWithBookingStatisticsQueryHandler : IRequestHandler<GetSessionsWithBookingStatisticsQueryInput, GetSessionsWithBookingStatisticsQueryOutput>
    {
        private readonly KinematikDbContext _dbContext;
        private readonly IFileStorageService _fileStorageService;

        public GetSessionsWithBookingStatisticsQueryHandler(
            KinematikDbContext dbContext,
            IFileStorageService fileStorageService
        )
        {
            _dbContext = dbContext;
            _fileStorageService = fileStorageService;
        }

        public async Task<GetSessionsWithBookingStatisticsQueryOutput> Handle(GetSessionsWithBookingStatisticsQueryInput request, CancellationToken cancellationToken)
        {
            GetSessionsWithBookingStatisticsQueryOutput output = new GetSessionsWithBookingStatisticsQueryOutput();

            output.Sessions = _dbContext.Sessions
                .Where(session => session.HallID.HasValue)
                .Select(session => new GetSessionsWithBookingStatisticsQueryOutput.MappedSession
                {
                    ID = session.ID,
                    FilmID = session.FilmID,
                    FilmTitle = session.Film.Title,
                    PosterUrl = _fileStorageService.GetAccessingPath(session.Film.PosterPath),
                    HallID = session.HallID.Value,
                    HallTitle = session.Hall.Title,
                    HallCapacity = session.Hall.LayoutItems.Count,
                    BookedSeatsQuantity = session.Bookings.SelectMany(booking => booking.BookedSeats).Count(),
                    StartAt = session.StartAt
                });

            return output;
        }
    }

    public class GetSessionsWithBookingStatisticsQueryOutput
    {
        public IEnumerable<MappedSession> Sessions { get; set; }

        public class MappedSession
        {
            public int ID { get; set; }
            public int FilmID { get; set; }
            public string FilmTitle { get; set; }
            public string? PosterUrl { get; set; }
            public int HallID { get; set; }
            public string HallTitle { get; set; }
            public int HallCapacity { get; set; }
            public int BookedSeatsQuantity { get; set; }
            public DateTime StartAt { get; set; }
        }
    }
}
