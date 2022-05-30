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
    public class BookingsController : HttpApiControllerBase
    {
        private readonly IMediator _mediator;

        public BookingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("{sessionID:int}")]
        [SwaggerOperation(
            Summary = "Повертає стан бронювань на зазначений сеанс"
        )]
        public async Task<ActionResult<GetBookingStatusesResponse>> GetBookingStatuses(int sessionID, CancellationToken cancellationToken = default)
        {
            GetBookingStatusesResponse response = new GetBookingStatusesResponse();

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
    }
}