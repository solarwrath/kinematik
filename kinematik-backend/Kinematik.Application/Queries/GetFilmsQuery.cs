using Kinematik.Domain.Entities;
using Kinematik.EntityFramework;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Kinematik.Application.Queries
{
    public class GetFilmsQueryRequest : IRequest<IEnumerable<Film>>
    {

    }

    public class GetFilmsQueryHandler : IRequestHandler<GetFilmsQueryRequest, IEnumerable<Film>>
    {
        private readonly KinematikDbContext _dbContext;

        public GetFilmsQueryHandler(KinematikDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Film>> Handle(GetFilmsQueryRequest request, CancellationToken cancellationToken)
        {
            IEnumerable<Film> films = await _dbContext.Films.ToArrayAsync(cancellationToken);

            return films;
        }
    }
}
