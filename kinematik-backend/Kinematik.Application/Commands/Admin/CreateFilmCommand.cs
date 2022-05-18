using Kinematik.Application.Ports;
using Kinematik.Domain.Entities;
using Kinematik.EntityFramework;

using MediatR;

namespace Kinematik.Application.Commands.Admin
{
    public class CreateFilmCommandInput : IRequest<Film>
    {
        public string Title { get; set; }
        public string? PosterImageFileName { get; set; }
        public byte[]? PosterImageContents { get; set; }
        public string Description { get; set; }
        public IEnumerable<int>? GenreIDs { get; set; }
        public int? Runtime { get; set; }
        public string? ImdbID { get; set; }
        public string? TrailerUrl { get; set; }
        public string? FeaturedImageFileName { get; set; }
        public byte[]? FeaturedImageContents { get; set; }
    }

    public class CreateFilmCommandHandler : IRequestHandler<CreateFilmCommandInput, Film>
    {
        private readonly KinematikDbContext _dbContext;
        private readonly IFileStorageService _fileStorageService;

        public CreateFilmCommandHandler(
            KinematikDbContext dbContext,
            IFileStorageService fileStorageService
        )
        {
            _dbContext = dbContext;
            _fileStorageService = fileStorageService;
        }

        public async Task<Film> Handle(CreateFilmCommandInput input, CancellationToken cancellationToken)
        {
            Film createdFilm = new Film
            {
                Title = input.Title,
                Description = input.Description,
                Runtime = input.Runtime,
                ImdbID = input.ImdbID,
                TrailerUrl = input.TrailerUrl
            };

            createdFilm.GenrePairs = input.GenreIDs
                .Select(filmGenreId => new FilmToGenrePair
                {
                    Film = createdFilm,
                    GenreID = filmGenreId
                })
                .ToArray();


            await _dbContext.AddAsync(createdFilm, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            bool hasPosterImage = input.PosterImageContents != null
                                  && input.PosterImageContents.Length > 0
                                  && !string.IsNullOrWhiteSpace(input.PosterImageFileName);
            if (hasPosterImage)
            {
                string posterStoragePath = $"Images/Posters/{createdFilm.ID}_{input.PosterImageFileName}";
                await _fileStorageService.StoreFileAsync(posterStoragePath, input.PosterImageContents!, cancellationToken);
                createdFilm.PosterPath = posterStoragePath;
            }

            bool hasFeaturedImage = input.FeaturedImageContents != null 
                                    && input.FeaturedImageContents.Length > 0
                                    && !string.IsNullOrWhiteSpace(input.FeaturedImageFileName);
            if (hasFeaturedImage)
            {
                string featuredImageStoragePath = $"Images/FeaturedImages/{createdFilm.ID}_{input.FeaturedImageFileName}";
                await _fileStorageService.StoreFileAsync(featuredImageStoragePath, input.FeaturedImageContents!, cancellationToken);
                createdFilm.FeaturedImagePath = featuredImageStoragePath;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return createdFilm;
        }
    }
}
