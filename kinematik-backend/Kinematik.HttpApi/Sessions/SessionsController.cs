using Kinematik.Application.Commands.Admin.Halls;
using Kinematik.Application.Queries.Admin.Halls;
using Kinematik.Application.Queries.Sessions;
using Kinematik.HttpApi.Sessions.GetSessions;
using Kinematik.HttpApi.Sessions.UpdateAllSessions;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

namespace Kinematik.HttpApi.Halls
{
    public class SessionsController : HttpApiControllerBase
    {
        private readonly IMediator _mediator;

        public SessionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Повертає всі сеанси"
        )]
        public async Task<ActionResult<GetAllSessionsResponse>> GetAllSessions(CancellationToken cancellationToken = default)
        {
            GetAllSessionsResponse response = new GetAllSessionsResponse();

            GetAllSessionsQueryOutput queryOutput = await _mediator.Send(new GetAllSessionsQueryInput(), cancellationToken);

            response.Sessions = queryOutput.Sessions.Select(session => new GetSessionsResponseMappedSession
            {
                ID = session.ID,
                FilmID = session.FilmID,
                HallID = session.HallID,
                StartAt = session.StartAt,
            });

            return Ok(response);
        }

        [HttpPut]
        [SwaggerOperation(
            Summary = "Оновлює всі сеанси"
        )]
        public async Task<ActionResult> UpdateAllSessions(
            UpdateAllSessionsRequest incomingRequest,
            CancellationToken cancellationToken = default
        )
        {
            UpdateAllSessionsCommandInput commandInput = new UpdateAllSessionsCommandInput
            {
                UpdatedSessions = incomingRequest.Sessions.Select(updatedSession => new UpdateAllSessionsCommandInputSession
                {
                    ID = updatedSession.ID,
                    FilmID = updatedSession.FilmID,
                    HallID = updatedSession.HallID,
                    StartAt = updatedSession.StartAt
                })
            };

            await _mediator.Send(commandInput, cancellationToken);

            return Ok();
        }
    }
}