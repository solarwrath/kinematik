using Kinematik.Domain.Entities;
using Kinematik.EntityFramework;

using MediatR;

namespace Kinematik.Application.Commands.Admin.Halls
{
    public class CreateHallCommandInput : IRequest<int>
    {
        public string Title { get; set; }
        public IEnumerable<LayoutItem> LayoutItems { get; set; }

        public class LayoutItem
        {
            public int RowID { get; set; }
            public int ColumnID { get; set; }
            public SeatType SeatType { get; set; }
        }
    }

    public class CreateHallCommandHandler : IRequestHandler<CreateHallCommandInput, int>
    {
        private readonly KinematikDbContext _dbContext;

        public CreateHallCommandHandler(
            KinematikDbContext dbContext
        )
        {
            _dbContext = dbContext;
        }

        public async Task<int> Handle(CreateHallCommandInput input, CancellationToken cancellationToken)
        {
            Hall createdHall = new Hall
            {
                Title = input.Title,
                LayoutItems = input.LayoutItems
                    .Select(rawLayoutItem => new HallLayoutItem
                    {
                        RowID = rawLayoutItem.RowID,
                        ColumnID = rawLayoutItem.ColumnID,
                        SeatType = rawLayoutItem.SeatType
                    })
                    .ToList()
            };

            _dbContext.Add(createdHall);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return createdHall.ID;
        }
    }
}