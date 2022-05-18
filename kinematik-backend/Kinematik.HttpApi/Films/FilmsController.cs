using Kinematik.Application.Commands.Admin;
using Kinematik.Application.Queries;
using Kinematik.Application.Queries.Admin;
using Kinematik.Domain.Entities;
using Kinematik.HttpApi.Films.Admin.CreateFilm;
using Kinematik.HttpApi.Films.Admin.GetFilmsList;
using Kinematik.HttpApi.Films.Admin.GetFullFilmData;
using Kinematik.HttpApi.Films.Admin.UpdateFilm;
using Kinematik.HttpApi.Films.GetFilmDetails;
using Kinematik.HttpApi.Films.GetRunningFilms;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using Swashbuckle.AspNetCore.Annotations;

namespace Kinematik.HttpApi.Films
{
    public class FilmsController : HttpApiControllerBase
    {
        private readonly IMediator _mediator;

        public FilmsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("list")]
        [SwaggerOperation(
            Summary = "Повертає список існуючих фільмів"
        )]
        public async Task<ActionResult<GetFilmsListResponse>> GetFilmsList(CancellationToken cancellationToken = default)
        {
            GetFilmsListQueryOutput queryOutput = await _mediator.Send(new GetFilmsListQueryInput(), cancellationToken);

            GetFilmsListResponse response = new GetFilmsListResponse
            {
                Films = queryOutput.Films.Select(originalFilm => new GetFilmsListResponseMappedFilm
                {
                    ID = originalFilm.ID,
                    Title = originalFilm.Title,
                    PosterUrl = originalFilm.PosterUrl,
                    Description = originalFilm.Description,
                    GenreIDs = originalFilm.GenreIDs
                })
            };

            return Ok(response);
        }

        [HttpGet]
        [Route("{filmID:int}")]
        [SwaggerOperation(
            Summary = "Повертає повну інформацію про фільм"
        )]
        public async Task<ActionResult<GetFullFilmDataResponse>> GetFullFilmData([FromRoute] int filmID, CancellationToken cancellationToken = default)
        {
            GetFullFilmDataQueryOutput queryOutput = await _mediator.Send(
                new GetFullFilmDataQueryInput()
                {
                    FilmID = filmID
                },
                cancellationToken
            );

            GetFullFilmDataResponse response = new GetFullFilmDataResponse
            {
                Title = queryOutput.Title,
                PosterUrl = queryOutput.PosterUrl,
                Description = queryOutput.Description,
                GenreIDs = queryOutput.GenreIDs,
                Runtime = queryOutput.Runtime,
                ImdbID = queryOutput.ImdbID,
                TrailerUrl = queryOutput.TrailerUrl,
                FeaturedImageUrl = queryOutput.FeaturedImageUrl
            };

            return Ok(response);
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Створює фільм"
        )]
        public async Task<ActionResult<Film>> CreateFilm(
            [FromForm] CreateFilmRequest incomingRequest,
            CancellationToken cancellationToken = default
        )
        {
            CreateFilmCommandInput commandInputInput = new CreateFilmCommandInput
            {
                Title = incomingRequest.Title,
                Description = incomingRequest.Description,
                GenreIDs = JsonConvert.DeserializeObject<int[]>(incomingRequest.SerializedGenreIDs),
                Runtime = JsonConvert.DeserializeObject<int>(incomingRequest.SerializedRuntime),
                ImdbID = incomingRequest.ImdbID,
                TrailerUrl = incomingRequest.TrailerUrl
            };

            if (incomingRequest.Poster != null)
            {
                commandInputInput.PosterImageFileName = incomingRequest.Poster.FileName;
                using (MemoryStream posterImageStream = new MemoryStream())
                {
                    await incomingRequest.Poster.CopyToAsync(posterImageStream, cancellationToken);
                    commandInputInput.PosterImageContents = posterImageStream.ToArray();
                }
            }

            if (incomingRequest.FeaturedImage != null)
            {
                commandInputInput.FeaturedImageFileName = incomingRequest.FeaturedImage.FileName;
                using (MemoryStream featuredImageStream = new MemoryStream())
                {
                    await incomingRequest.FeaturedImage.CopyToAsync(featuredImageStream, cancellationToken);
                    commandInputInput.FeaturedImageContents = featuredImageStream.ToArray();
                }
            }

            Film createdFilm = await _mediator.Send(commandInputInput, cancellationToken);

            return Ok(createdFilm);
        }

        [HttpPut]
        [Route("{filmID:int}")]
        [SwaggerOperation(
            Summary = "Редагує фільм"
        )]
        public async Task<ActionResult<Film>> UpdateFilm(
            [FromRoute] int filmID,
            [FromForm] UpdateFilmRequest incomingRequest,
            CancellationToken cancellationToken = default
        )
        {
            UpdateFilmCommandInput commandInput = new UpdateFilmCommandInput
            {
                FilmID = filmID,
                Title = incomingRequest.Title,
                Description = incomingRequest.Description,
                GenreIDs = JsonConvert.DeserializeObject<int[]>(incomingRequest.SerializedGenreIDs),
                Runtime = JsonConvert.DeserializeObject<int>(incomingRequest.SerializedRuntime),
                ImdbID = incomingRequest.ImdbID,
                TrailerUrl = incomingRequest.TrailerUrl,
                WasPosterImageDeleted = JsonConvert.DeserializeObject<bool>(incomingRequest.SerializedWasPosterDeleted),
                WasFeaturedImageDeleted = JsonConvert.DeserializeObject<bool>(incomingRequest.SerializedWasFeaturedImageDeleted)
            };

            if (incomingRequest.Poster != null)
            {
                commandInput.PosterImageFileName = incomingRequest.Poster.FileName;
                using (MemoryStream posterImageStream = new MemoryStream())
                {
                    await incomingRequest.Poster.CopyToAsync(posterImageStream, cancellationToken);
                    commandInput.PosterImageContents = posterImageStream.ToArray();
                }
            }

            if (incomingRequest.FeaturedImage != null)
            {
                commandInput.FeaturedImageFileName = incomingRequest.FeaturedImage.FileName;
                using (MemoryStream featuredImageStream = new MemoryStream())
                {
                    await incomingRequest.FeaturedImage.CopyToAsync(featuredImageStream, cancellationToken);
                    commandInput.FeaturedImageContents = featuredImageStream.ToArray();
                }
            }

            Film updatedFilm = await _mediator.Send(commandInput, cancellationToken);

            return Ok(updatedFilm);
        }

        [HttpDelete]
        [Route("{filmID:int}")]
        [SwaggerOperation(
            Summary = "Видаляє фільм"
        )]
        public async Task<ActionResult> DeleteFilm( [FromRoute] int filmID, CancellationToken cancellationToken = default)
        {
            DeleteFilmCommandInput commandInput = new DeleteFilmCommandInput
            {
                FilmID = filmID
            };

            await _mediator.Send(commandInput, cancellationToken);

            return Ok();
        }

        [HttpGet]
        [Route("running")]
        [SwaggerOperation(
            Summary = "Повертає інформацію про актуальну кіноафішу"
        )]
        public async Task<ActionResult<GetRunningFilmsResponse>> GetRunningFilms(CancellationToken cancellationToken = default)
        {
            GetRunningFilmsQueryOutput queryOutput = await _mediator.Send(new GetPlayingFilmsQueryInput(), cancellationToken);

            GetRunningFilmsResponse response = new GetRunningFilmsResponse
            {
                RunningFilms = queryOutput.RunningFilms.Select(film => new GetRunningFilmsResponseFilm
                {
                    ID = film.ID,
                    Title = film.Title,
                    PosterUrl = film.PosterUrl
                })
            };

            return Ok(response);
        }
        
        [HttpGet]
        [Route("{filmID:int}/details")]
        [SwaggerOperation(
            Summary = "Повертає детальну інформацію про фільм"
        )]
        public async Task<ActionResult<GetFilmDetailsResponse>> GetFilmDetails([FromRoute]int filmID, CancellationToken cancellationToken = default)
        {
            GetFilmDetailsQueryOutput queryOutput = await _mediator.Send(
                new GetFilmDetailsQueryInput
                {
                    FilmID = filmID
                },
                cancellationToken
            );

            GetFilmDetailsResponse response = new GetFilmDetailsResponse
            {
                Title = queryOutput.Title,
                PosterUrl = queryOutput.PosterUrl,
                Description = queryOutput.Description,
                GenreIDs = queryOutput.GenreIDs,
                Runtime = queryOutput.Runtime,
                Rating = queryOutput.Rating,
                TrailerUrl = queryOutput.TrailerUrl,
                FeaturedImageUrl = queryOutput.FeaturedImageUrl
            };

            return Ok(response);
        }
    }
}