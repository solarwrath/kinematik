using Kinematik.Domain.Entities;
using Kinematik.EntityFramework;

using MediatR;

namespace Kinematik.Application.Commands.Admin.Halls
{
    public class DeleteHallCommandInput : IRequest
    {
        public int HallID { get; set; }
    }

    public class DeleteHallCommandHandler : IRequestHandler<DeleteHallCommandInput>
    {
        private readonly KinematikDbContext _dbContext;

        public DeleteHallCommandHandler(
            KinematikDbContext dbContext
        )
        {
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(DeleteHallCommandInput input, CancellationToken cancellationToken)
        {
            Hall? hall = await _dbContext.Halls.FindAsync(new object[] { input.HallID }, cancellationToken);

            _dbContext.Halls.Remove(hall);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}