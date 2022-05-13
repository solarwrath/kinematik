using Kinematik_Domain;

using Kinematik_EntityFramework;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Kinematik_Application.Queries
{
    public class GetFilmsQueryRequest : IRequest<IEnumerable<Film>>
    {

    }

    public class GetFilmsQueryHandler : IRequestHandler<GetFilmsQueryRequest, IEnumerable<Film>>
    {
        private readonly KinematikDbContext _dbContext;

        public GetFilmsQueryHandler(KinematikDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<IEnumerable<Film>> Handle(GetFilmsQueryRequest request, CancellationToken cancellationToken)
        {
            IEnumerable<Film> films = await this._dbContext.Films.ToArrayAsync(cancellationToken);

            return films;
        }
    }
}
