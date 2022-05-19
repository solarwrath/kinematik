using Kinematik.Domain.Entities;
using Kinematik.EntityFramework;

using MediatR;

namespace Kinematik.Application.Commands.Admin.Halls
{
    public class UpdateHallCommandInput : IRequest
    {
        public int HallID { get; set; }
        public string UpdatedTitle { get; set; }
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
            Hall? hall = await _dbContext.Halls.FindAsync(new object[] { input.HallID }, cancellationToken);
            hall.Title = input.UpdatedTitle;
            
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}