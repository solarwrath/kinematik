using Kinematik.EntityFramework;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Kinematik.Application.Queries.Admin.Halls
{
    public class GetHallsListQueryInput : IRequest<GetHallsListQueryOutput>
    {
    }

    public class GetHallsListQueryHandler : IRequestHandler<GetHallsListQueryInput, GetHallsListQueryOutput>
    {
        private readonly KinematikDbContext _dbContext;

        public GetHallsListQueryHandler(
            KinematikDbContext dbContext
        )
        {
            _dbContext = dbContext;
        }

        public async Task<GetHallsListQueryOutput> Handle(GetHallsListQueryInput request, CancellationToken cancellationToken)
        {
            GetHallsListQueryOutput output = new GetHallsListQueryOutput();

            var halls = await _dbContext.Halls
                .Select(hall => new
                {
                    hall.ID,
                    hall.Title
                })
                .ToArrayAsync(cancellationToken);

            output.Halls = halls
                .Select(film =>
                {
                    return new GetHallsListQueryOutput.MappedHall
                    {
                        ID = film.ID,
                        Title = film.Title
                    };
                });

            return output;
        }
    }

    public class GetHallsListQueryOutput
    {
        public IEnumerable<MappedHall> Halls { get; set; }

        public class MappedHall
        {
            public int ID { get; set; }
            public string Title { get; set; } = null!;
        }
    }
}
