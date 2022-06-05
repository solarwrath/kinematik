using Kinematik.Application.Ports;
using Kinematik.EntityFramework;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Kinematik.Application.Queries.Admin.Films
{
    public class GetFilmsListQueryInput : IRequest<GetFilmsListQueryOutput>
    {
    }

    public class GetFilmsListQueryHandler : IRequestHandler<GetFilmsListQueryInput, GetFilmsListQueryOutput>
    {
        private readonly KinematikDbContext _dbContext;
        private readonly IFileStorageService _fileStorageService;

        public GetFilmsListQueryHandler(
            KinematikDbContext dbContext,
            IFileStorageService fileStorageService
        )
        {
            _dbContext = dbContext;
            _fileStorageService = fileStorageService;
        }

        public async Task<GetFilmsListQueryOutput> Handle(GetFilmsListQueryInput request, CancellationToken cancellationToken)
        {
            GetFilmsListQueryOutput output = new GetFilmsListQueryOutput();

            var films = await _dbContext.Films
                .Select(film => new
                {
                    film.ID,
                    film.Title,
                    film.PosterPath,
                    film.Description,
                    film.Runtime,
                    GenreIDs = film.GenrePairs.Select(genrePair => genrePair.GenreID)
                })
                .ToArrayAsync(cancellationToken);

            output.Films = films
                .Select(film =>
                {
                    string? posterUrl = !string.IsNullOrWhiteSpace(film.PosterPath)
                        ? _fileStorageService.GetAccessingPath(film.PosterPath)
                        : null;

                    return new GetFilmsListQueryOutput.MappedFilm
                    {
                        ID = film.ID,
                        Title = film.Title,
                        PosterUrl = posterUrl,
                        Description = film.Description,
                        Runtime = film.Runtime,
                        GenreIDs = film.GenreIDs
                    };
                });

            return output;
        }
    }

    public class GetFilmsListQueryOutput
    {
        public IEnumerable<MappedFilm> Films { get; set; }

        public class MappedFilm
        {
            public int ID { get; set; }
            public string Title { get; set; } = null!;
            public string? PosterUrl { get; set; }
            public string Description { get; set; } = null!;
            public int? Runtime { get; set; }
            public IEnumerable<int>? GenreIDs { get; set; }
        }
    }
}
