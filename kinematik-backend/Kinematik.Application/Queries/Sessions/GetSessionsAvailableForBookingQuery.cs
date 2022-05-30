using Kinematik.EntityFramework;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Kinematik.Application.Queries.Sessions
{
    public class GetSessionsAvailableForBookingQueryInput : IRequest<GetSessionsAvailableForBookingQueryOutput>
    {
        public int FilmID { get; set; }
    }

    public class GetSessionsAvailableForBookingQueryHandler : IRequestHandler<GetSessionsAvailableForBookingQueryInput, GetSessionsAvailableForBookingQueryOutput>
    {
        private readonly KinematikDbContext _dbContext;

        public GetSessionsAvailableForBookingQueryHandler(
            KinematikDbContext dbContext
        )
        {
            _dbContext = dbContext;
        }

        public async Task<GetSessionsAvailableForBookingQueryOutput> Handle(GetSessionsAvailableForBookingQueryInput request, CancellationToken cancellationToken)
        {
            GetSessionsAvailableForBookingQueryOutput output = new GetSessionsAvailableForBookingQueryOutput();

            output.Sessions = await _dbContext.Sessions
                .Where(session => session.FilmID == request.FilmID)
                .Where(session => session.HallID.HasValue)
                .Select(session => new GetSessionsAvailableForBookingQueryOutput.MappedSession
                {
                    ID = session.ID,
                    HallID = session.HallID.Value,
                    HallTitle = session.Hall.Title,
                    StartAt = session.StartAt
                })
                .ToArrayAsync(cancellationToken);

            return output;
        }
    }

    public class GetSessionsAvailableForBookingQueryOutput
    {
        public IEnumerable<MappedSession> Sessions { get; set; }

        public class MappedSession
        {
            public int ID { get; set; }
            public int HallID { get; set; }
            public string HallTitle { get; set; }
            public DateTime StartAt { get; set; }
        }
    }
}