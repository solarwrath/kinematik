using System.Globalization;

using Kinematik.Application.Ports;
using Kinematik.Application.ThirdParty.ImdbAPI;
using Kinematik.EntityFramework;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

namespace Kinematik.Application.Queries
{
    public class GetFilmDetailsQueryInput : IRequest<GetFilmDetailsQueryOutput>
    {
        public int FilmID { get; set; }
    }

    public class GetFilmDetailsQueryHandler : IRequestHandler<GetFilmDetailsQueryInput, GetFilmDetailsQueryOutput>
    {
        private readonly KinematikDbContext _dbContext;
        private readonly IFileStorageService _fileStorageService;
        private readonly IHttpClientFactory _httpClientFactory;

        public GetFilmDetailsQueryHandler(
            KinematikDbContext dbContext,
            IFileStorageService fileStorageService,
            IHttpClientFactory httpClientFactory
        )
        {
            _dbContext = dbContext;
            _fileStorageService = fileStorageService;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<GetFilmDetailsQueryOutput> Handle(GetFilmDetailsQueryInput request, CancellationToken cancellationToken)
        {
            GetFilmDetailsQueryOutput output = new GetFilmDetailsQueryOutput();

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
            output.TrailerUrl = film.TrailerUrl;
            output.FeaturedImageUrl = _fileStorageService.GetAccessingPath(film.FeaturedImagePath);
            output.GenreIDs = film.GenreIDs;

            if (!string.IsNullOrWhiteSpace(film.ImdbID))
            {
                string imdbApiKey = Environment.GetEnvironmentVariable("IMDB_API_KEY");
                string endpointUrl = $"https://imdb-api.com/en/API/Ratings/{imdbApiKey}/{film.ImdbID}";

                HttpClient httpClient = _httpClientFactory.CreateClient();
                Stream responseStream = await httpClient.GetStreamAsync(endpointUrl, cancellationToken);

                using (StreamReader responseStringReader = new StreamReader(responseStream))
                {
                    using (JsonTextReader responseJsonReader = new JsonTextReader(responseStringReader))
                    {
                        ImdbRatingResponse imdbRating = new JsonSerializer().Deserialize<ImdbRatingResponse>(responseJsonReader);
                        output.Rating = decimal.Parse(imdbRating.IMDb, new NumberFormatInfo { NumberDecimalSeparator = "." });
                    }
                }
            }

            return output;
        }
    }

    public class GetFilmDetailsQueryOutput
    {
        public string Title { get; set; } = null!;
        public string? PosterUrl { get; set; }
        public string Description { get; set; } = null!;
        public IEnumerable<int>? GenreIDs { get; set; }
        public int? Runtime { get; set; }
        public decimal? Rating { get; set; }
        public string? TrailerUrl { get; set; }
        public string? FeaturedImageUrl { get; set; }
    }
}
