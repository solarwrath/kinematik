using Kinematik.Application.Ports;
using Kinematik.Domain.Entities;
using Kinematik.EntityFramework;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Kinematik.Application.Queries.Films
{
    public class GetPlayingFilmsQueryInput : IRequest<GetRunningFilmsQueryOutput>
    {

    }

    public class GetPlayingFilmsQueryHandler : IRequestHandler<GetPlayingFilmsQueryInput, GetRunningFilmsQueryOutput>
    {
        private readonly KinematikDbContext _dbContext;
        private readonly IFileStorageService _fileStorageService;

        public GetPlayingFilmsQueryHandler(
            KinematikDbContext dbContext,
            IFileStorageService fileStorageService
        )
        {
            _dbContext = dbContext;
            _fileStorageService = fileStorageService;
        }

        public async Task<GetRunningFilmsQueryOutput> Handle(GetPlayingFilmsQueryInput request, CancellationToken cancellationToken)
        {
            GetRunningFilmsQueryOutput output = new GetRunningFilmsQueryOutput();

            IEnumerable<Film> unmappedFilms = await _dbContext.Films.ToArrayAsync(cancellationToken);

            output.RunningFilms = unmappedFilms.Select(unmappedFilm => new GetRunningFilmsQueryOutput.RunningFilm
            {
                ID = unmappedFilm.ID,
                Title = unmappedFilm.Title,
                PosterUrl = _fileStorageService.GetAccessingPath(unmappedFilm.PosterPath),
            });

            return output;
        }
    }

    public class GetRunningFilmsQueryOutput
    {
        public IEnumerable<RunningFilm> RunningFilms { get; set; }

        public class RunningFilm
        {
            public int ID { get; set; }
            public string Title { get; set; }
            public string? PosterUrl { get; set; }
        }
    }
}