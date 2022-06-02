using Kinematik.Domain.Entities;
using Kinematik.EntityFramework;

using MediatR;
using Microsoft.EntityFrameworkCore;

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

            var hall = await _dbContext.Halls
                .Where(hall => hall.ID == request.HallID)
                .Select(hall =>
                    new
                    {
                        hall.Title,
                        HallLayoutItems = hall.LayoutItems.Select(hallLayoutItem => new GetHallQueryOutput.MappedHallLayoutItem
                        {
                            RowID = hallLayoutItem.RowID,
                            ColumnID = hallLayoutItem.ColumnID,
                            SeatType = hallLayoutItem.SeatType,
                        })
                    })
                .SingleOrDefaultAsync(cancellationToken);

            output.Title = hall.Title;
            output.LayoutItems = hall.HallLayoutItems;

            return output;
        }
    }

    public class GetHallQueryOutput
    {
        public string Title { get; set; }
        public IEnumerable<MappedHallLayoutItem> LayoutItems { get; set; }

        public class MappedHallLayoutItem
        {
            public int RowID { get; set; }
            public int ColumnID { get; set; }
            public SeatType SeatType { get; set; }
        }
    }
}
