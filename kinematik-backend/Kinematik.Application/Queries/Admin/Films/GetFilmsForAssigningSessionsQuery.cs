using Kinematik.Application.Ports;
using Kinematik.EntityFramework;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Kinematik.Application.Queries.Admin.Films
{
    public class GetFilmsForAssigningSessionsQueryInput : IRequest<GetFilmsForAssigningSessionsQueryOutput>
    {
    }

    public class GetFilmsForAssigningSessionsQueryHandler : IRequestHandler<GetFilmsForAssigningSessionsQueryInput, GetFilmsForAssigningSessionsQueryOutput>
    {
        private readonly KinematikDbContext _dbContext;

        public GetFilmsForAssigningSessionsQueryHandler(
            KinematikDbContext dbContext
        )
        {
            _dbContext = dbContext;
        }

        public async Task<GetFilmsForAssigningSessionsQueryOutput> Handle(GetFilmsForAssigningSessionsQueryInput request, CancellationToken cancellationToken)
        {
            GetFilmsForAssigningSessionsQueryOutput output = new GetFilmsForAssigningSessionsQueryOutput();

            output.Films = await _dbContext.Films
                .Where(film => film.Runtime.HasValue)
                .Select(film => new GetFilmsForAssigningSessionsQueryMappedFilm
                {
                    ID = film.ID,
                    Title = film.Title,
                    Runtime = film.Runtime.Value
                })
                .ToArrayAsync(cancellationToken);

            return output;
        }
    }

    public class GetFilmsForAssigningSessionsQueryOutput
    {
        public IEnumerable<GetFilmsForAssigningSessionsQueryMappedFilm> Films { get; set; }
    }

    public class GetFilmsForAssigningSessionsQueryMappedFilm
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int Runtime { get; set; }
    }
}