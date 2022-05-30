using Kinematik.Application.Commands.Admin.Sessions;
using Kinematik.Application.Queries.Sessions;
using Kinematik.HttpApi.Sessions.GetSessions;
using Kinematik.HttpApi.Sessions.GetSessionsAvailableForBooking;
using Kinematik.HttpApi.Sessions.UpdateAllSessions;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

namespace Kinematik.HttpApi.Sessions
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

        [HttpGet]
        [Route("available-for-booking/{filmID:int}")]
        [SwaggerOperation(
            Summary = "Повертає всі сеанси, на які можна взяти квиток на зазначений фільм"
        )]
        public async Task<ActionResult<GetSessionsAvailableForBookingResponse>> GetSessionsAvailableForBooking(
            [FromRoute] int filmID,
            CancellationToken cancellationToken = default
        )
        {
            GetSessionsAvailableForBookingResponse response = new GetSessionsAvailableForBookingResponse();

            GetSessionsAvailableForBookingQueryInput queryInput = new GetSessionsAvailableForBookingQueryInput
            {
                FilmID = filmID
            };
            GetSessionsAvailableForBookingQueryOutput queryOutput = await _mediator.Send(queryInput, cancellationToken);

            response.Sessions = queryOutput.Sessions.Select(session => new GetSessionsAvailableForBookingMappedSession
            {
                ID = session.ID,
                HallID = session.HallID,
                HallTitle = session.HallTitle,
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