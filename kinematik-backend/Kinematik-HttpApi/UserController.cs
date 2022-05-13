using Kinematik_Application.Commands;
using Kinematik_Application.Queries;
using Kinematik_Domain;
using Kinematik_EntityFramework;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Kinematik_HttpApi
{
    public class UserController : HttpApiControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Створює фільм"
        )]
        public async Task<ActionResult<Film>> CreateFilm(CancellationToken cancellationToken = default)
        {
            Film createdFilm = await this._mediator.Send(new CreateFilmCommandRequest(), cancellationToken);

            return Ok(createdFilm);
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Дістає фільми"
        )]
        public async Task<ActionResult<IEnumerable<Film>>> GetFilms(CancellationToken cancellationToken = default)
        {
            IEnumerable<Film> films = await this._mediator.Send(new GetFilmsQueryRequest(), cancellationToken);

            return Ok(films);
        }
    }
}