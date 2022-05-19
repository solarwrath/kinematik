using Kinematik.Domain.Entities;
using Kinematik.EntityFramework;

using MediatR;

namespace Kinematik.Application.Commands.Admin.Halls
{
    public class CreateHallCommandInput : IRequest<int>
    {
        public string Title { get; set; }
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
                Title = input.Title
            };

            _dbContext.Add(createdHall);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return createdHall.ID;
        }
    }
}