using Kinematik.Domain.Entities;
using Kinematik.EntityFramework;

using MediatR;

namespace Kinematik.Application.Commands.Admin.Films
{
    public class DeleteFilmCommandInput : IRequest
    {
        public int FilmID { get; set; }
    }

    public class DeleteFilmCommandHandler : IRequestHandler<DeleteFilmCommandInput>
    {
        private readonly KinematikDbContext _dbContext;

        public DeleteFilmCommandHandler(
            KinematikDbContext dbContext
        )
        {
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(DeleteFilmCommandInput input, CancellationToken cancellationToken)
        {
            Film? film = await _dbContext.Films.FindAsync(new object[] { input.FilmID }, cancellationToken);
            _dbContext.Films.Remove(film);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}