using Kinematik.Domain.Entities;
using Kinematik.EntityFramework;

using MediatR;

namespace Kinematik.Application.Commands
{
    public class CreateFilmCommandRequest : IRequest<Film>
    {

    }

    public class CreateFilmCommandHandler: IRequestHandler<CreateFilmCommandRequest, Film>
    {
        private readonly KinematikDbContext _dbContext;

        public CreateFilmCommandHandler(KinematikDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<Film> Handle(CreateFilmCommandRequest request, CancellationToken cancellationToken)
        {
            Film createdFilm = new Film
            {
                Title = "Vladislavius Finyk"
            };
            await _dbContext.AddAsync(createdFilm, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return createdFilm;
        }
    }
}
