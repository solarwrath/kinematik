
using Kinematik.Application.Ports;
using Kinematik.EntityFramework;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Kinematik.Application.Queries
{
    public class GetFullFilmDataQueryInput : IRequest<GetFullFilmDataQueryOutput>
    {
        public int FilmID { get; set; }
    }

    public class GetFullFilmDataQueryHandler : IRequestHandler<GetFullFilmDataQueryInput, GetFullFilmDataQueryOutput>
    {
        private readonly KinematikDbContext _dbContext;
        private readonly IFileStorageService _fileStorageService;

        public GetFullFilmDataQueryHandler(
            KinematikDbContext dbContext,
            IFileStorageService fileStorageService
        )
        {
            _dbContext = dbContext;
            _fileStorageService = fileStorageService;
        }

        public async Task<GetFullFilmDataQueryOutput> Handle(GetFullFilmDataQueryInput request, CancellationToken cancellationToken)
        {
            GetFullFilmDataQueryOutput output = new GetFullFilmDataQueryOutput();

            // TODO Introduce Language
            var film = await _dbContext.Films
                .Where(film => film.ID == request.FilmID)
                .Select(film => new
                {
                    film.Title,
                    film.PosterPath,
                    film.Description,
                    film.ImdbID,
                    film.Runtime,
                    film.TrailerUrl,
                    film.FeaturedImagePath,
                    GenreIDs = film.GenrePairs.Select(genrePair => genrePair.GenreID)
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (film == null)
            {
                // TODO Handle not existing Film
            }

            output.Title = film.Title;
            output.PosterUrl = _fileStorageService.GetAccessingPath(film.PosterPath);
            output.Description = film.Description;
            output.Runtime = film.Runtime;
            output.ImdbID = film.ImdbID;
            output.TrailerUrl = film.TrailerUrl;
            output.FeaturedImageUrl = _fileStorageService.GetAccessingPath(film.FeaturedImagePath);
            output.GenreIDs = film.GenreIDs;

            return output;
        }
    }

    public class GetFullFilmDataQueryOutput
    {
        public int ID { get; set; }
        public string Title { get; set; } = null!;
        public string? PosterUrl { get; set; }
        public string Description { get; set; } = null!;
        public IEnumerable<int>? GenreIDs { get; set; }
        public int? Runtime { get; set; }
        public string? ImdbID { get; set; }
        public string? TrailerUrl { get; set; }
        public string? FeaturedImageUrl { get; set; }
    }
}
