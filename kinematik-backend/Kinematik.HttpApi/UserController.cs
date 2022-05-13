using Kinematik.Application.Commands;
using Kinematik.Application.Queries;
using Kinematik.Domain.Entities;
using MediatR;

using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

namespace Kinematik.HttpApi
{
    public class UserController : HttpApiControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Створює фільм"
        )]
        public async Task<ActionResult<Film>> CreateFilm(CancellationToken cancellationToken = default)
        {
            Film createdFilm = await _mediator.Send(new CreateFilmCommandRequest(), cancellationToken);

            return Ok(createdFilm);
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Дістає фільми"
        )]
        public async Task<ActionResult<IEnumerable<Film>>> GetFilms(CancellationToken cancellationToken = default)
        {
            IEnumerable<Film> films = await _mediator.Send(new GetFilmsQueryRequest(), cancellationToken);

            return Ok(films);
        }
    }
}