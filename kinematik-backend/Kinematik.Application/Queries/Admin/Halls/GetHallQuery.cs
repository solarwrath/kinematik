using Kinematik.Domain.Entities;
using Kinematik.EntityFramework;

using MediatR;

namespace Kinematik.Application.Queries.Admin.Halls
{
    public class GetHallQueryInput : IRequest<GetHallQueryOutput>
    {
        public int HallID { get; set; }
    }

    public class GetHallQueryHandler : IRequestHandler<GetHallQueryInput, GetHallQueryOutput>
    {
        private readonly KinematikDbContext _dbContext;

        public GetHallQueryHandler(
            KinematikDbContext dbContext
        )
        {
            _dbContext = dbContext;
        }

        public async Task<GetHallQueryOutput> Handle(GetHallQueryInput request, CancellationToken cancellationToken)
        {
            GetHallQueryOutput output = new GetHallQueryOutput();

            Hall? hall = await _dbContext.Halls.FindAsync(new object[] {request.HallID}, cancellationToken);

            output.Title = hall.Title;

            return output;
        }
    }

    public class GetHallQueryOutput
    {
        public string Title { get; set; }
    }
}
