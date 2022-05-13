using Kinematik_Domain;
using Kinematik_EntityFramework;
using MediatR;

namespace Kinematik_Application.Commands
{
    public class CreateFilmCommandRequest : IRequest<Film>
    {

    }

    public class CreateFilmCommandHandler: IRequestHandler<CreateFilmCommandRequest, Film>
    {
        private readonly KinematikDbContext _dbContext;

        public CreateFilmCommandHandler(KinematikDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        
        public async Task<Film> Handle(CreateFilmCommandRequest request, CancellationToken cancellationToken)
        {
            Film createdFilm = new Film
            {
                Title = "Vladislavius Finyk"
            };
            await this._dbContext.AddAsync(createdFilm, cancellationToken);
            await this._dbContext.SaveChangesAsync(cancellationToken);

            return createdFilm;
        }
    }
}
