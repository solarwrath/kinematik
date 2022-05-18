using Kinematik.Application.Ports;
using Kinematik.Domain.Entities;
using Kinematik.EntityFramework;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Kinematik.Application.Commands.Admin
{
    public class UpdateFilmCommandInput : IRequest<Film>
    {
        public int FilmID { get; set; }
        public string Title { get; set; }
        public bool WasPosterImageDeleted { get; set; }
        public string? PosterImageFileName { get; set; }
        public byte[]? PosterImageContents { get; set; }
        public string Description { get; set; }
        public IEnumerable<int>? GenreIDs { get; set; }
        public int? Runtime { get; set; }
        public string? ImdbID { get; set; }
        public string? TrailerUrl { get; set; }
        public bool WasFeaturedImageDeleted { get; set; }
        public string? FeaturedImageFileName { get; set; }
        public byte[]? FeaturedImageContents { get; set; }
    }

    public class UpdateFilmCommandHandler : IRequestHandler<UpdateFilmCommandInput, Film>
    {
        private readonly KinematikDbContext _dbContext;
        private readonly IFileStorageService _fileStorageService;

        public UpdateFilmCommandHandler(
            KinematikDbContext dbContext,
            IFileStorageService fileStorageService
        )
        {
            _dbContext = dbContext;
            _fileStorageService = fileStorageService;
        }

        public async Task<Film> Handle(UpdateFilmCommandInput input, CancellationToken cancellationToken)
        {
            Film film = await _dbContext.Films
                .Include(film => film.GenrePairs)
                .Where(film => film.ID == input.FilmID)
                .SingleOrDefaultAsync(cancellationToken);

            film.Title = input.Title;

            if (input.WasPosterImageDeleted)
            {
                film.PosterPath = null;
            }
            else
            {
                bool wasPosterImageAdded = input.PosterImageContents != null
                                      && input.PosterImageContents.Length > 0
                                      && !string.IsNullOrWhiteSpace(input.PosterImageFileName);
                if (wasPosterImageAdded)
                {
                    string posterStoragePath = $"Images/Posters/{film.ID}_{input.PosterImageFileName}";
                    await _fileStorageService.StoreFileAsync(posterStoragePath, input.PosterImageContents!, cancellationToken);
                    film.PosterPath = posterStoragePath;
                }
            }

            film.Description = input.Description;
            film.ImdbID = input.ImdbID;
            film.Runtime = input.Runtime;
            film.TrailerUrl = input.TrailerUrl;

            if (input.WasFeaturedImageDeleted)
            {
                film.FeaturedImagePath = null;
            }
            else
            {
                bool wasFeaturedImageAdded = input.FeaturedImageContents != null
                                        && input.FeaturedImageContents.Length > 0
                                        && !string.IsNullOrWhiteSpace(input.FeaturedImageFileName);
                if (wasFeaturedImageAdded)
                {
                    string featuredImageStoragePath = $"Images/FeaturedImages/{film.ID}_{input.FeaturedImageFileName}";
                    await _fileStorageService.StoreFileAsync(featuredImageStoragePath, input.FeaturedImageContents!, cancellationToken);
                    film.FeaturedImagePath = featuredImageStoragePath;
                }
            }

            IEnumerable<int> previousGenreIDs = film.GenrePairs.Select(genrePair => genrePair.GenreID);
            IEnumerable<int> removedGenreIDs = previousGenreIDs.Except(input.GenreIDs);
            IEnumerable<int> addedGenreIDs = input.GenreIDs.Except(previousGenreIDs);

            IEnumerable<FilmToGenrePair> removedPairs = film.GenrePairs.Where(genrePair => removedGenreIDs.Contains(genrePair.GenreID));
            _dbContext.FilmToGenrePairs.RemoveRange(removedPairs);

            IEnumerable<FilmToGenrePair> addedPairs = addedGenreIDs.Select(filmGenreId => new FilmToGenrePair
            {
                Film = film,
                GenreID = filmGenreId
            });
            await _dbContext.FilmToGenrePairs.AddRangeAsync(addedPairs, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return film;
        }
    }
}
