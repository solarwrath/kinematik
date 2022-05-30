using Kinematik.Domain.Entities;
using Kinematik.EntityFramework;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Kinematik.Application.Commands.Admin.Halls
{
    public class UpdateHallCommandInput : IRequest
    {
        public int HallID { get; set; }
        public string UpdatedTitle { get; set; }
        public IEnumerable<UpdateHallCommandInput.LayoutItem> UpdatedLayoutItems { get; set; }

        public class LayoutItem
        {
            public int RowID { get; set; }
            public int ColumnID { get; set; }
            public HallLayoutItemType Type { get; set; }
        }
    }

    public class UpdateHallCommandHandler : IRequestHandler<UpdateHallCommandInput>
    {
        private readonly KinematikDbContext _dbContext;

        public UpdateHallCommandHandler(
            KinematikDbContext dbContext
        )
        {
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(UpdateHallCommandInput input, CancellationToken cancellationToken)
        {
            Hall? hall = await _dbContext.Halls
                .Where(hall => hall.ID == input.HallID)
                .Include(hall => hall.LayoutItems)
                .SingleOrDefaultAsync(cancellationToken);

            hall.Title = input.UpdatedTitle;

            _dbContext.HallLayoutItems.RemoveRange(hall.LayoutItems);
            hall.LayoutItems = input.UpdatedLayoutItems
                .Select(rawLayoutItem => new HallLayoutItem
                {
                    RowID = rawLayoutItem.RowID,
                    ColumnID = rawLayoutItem.ColumnID,
                    Type = rawLayoutItem.Type
                })
                .ToList();

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}