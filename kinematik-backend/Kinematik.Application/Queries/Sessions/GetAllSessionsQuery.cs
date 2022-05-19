using Kinematik.EntityFramework;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Kinematik.Application.Queries.Sessions
{
    public class GetAllSessionsQueryInput : IRequest<GetAllSessionsQueryOutput>
    {

    }

    public class GetAllSessionsQueryHandler : IRequestHandler<GetAllSessionsQueryInput, GetAllSessionsQueryOutput>
    {
        private readonly KinematikDbContext _dbContext;

        public GetAllSessionsQueryHandler(
            KinematikDbContext dbContext
        )
        {
            _dbContext = dbContext;
        }

        public async Task<GetAllSessionsQueryOutput> Handle(GetAllSessionsQueryInput request, CancellationToken cancellationToken)
        {
            GetAllSessionsQueryOutput output = new GetAllSessionsQueryOutput();

            output.Sessions = await _dbContext.Sessions
                .Select(session => new GetAllSessionsQueryOutput.MappedSession
                {
                    ID = session.ID,
                    FilmID = session.FilmID,
                    HallID = session.HallID,
                    StartAt = session.StartAt
                })
                .ToArrayAsync(cancellationToken);

            return output;
        }
    }

    public class GetAllSessionsQueryOutput
    {
        public IEnumerable<MappedSession> Sessions { get; set; }

        public class MappedSession
        {
            public int ID { get; set; }
            public int FilmID { get; set; }
            public int? HallID { get; set; }
            public DateTime StartAt { get; set; }
        }
    }
}