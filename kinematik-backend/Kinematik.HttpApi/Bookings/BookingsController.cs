using Kinematik.Application.Commands.Bookings;
using Kinematik.Application.Queries.Bookings;
using Kinematik.HttpApi.Bookings.CreateBooking;
using Kinematik.HttpApi.Bookings.GetAllSessionsResponse;

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
        public async Task<ActionResult<GetBookingStatusesResponse>> GetBookingStatuses(
            [FromRoute] int sessionID,
            CancellationToken cancellationToken = default
        )
        {
            GetBookingStatusesResponse response = new GetBookingStatusesResponse();

            GetBookingStatusesQueryOutput queryOutput = await _mediator.Send(
                new GetBookingStatusesQueryInput
                {
                    SessionID = sessionID
                },
                cancellationToken
            );

            response.BookingStatuses = queryOutput.BookingStatuses.Select(bookingStatus => new GetBookingStatusesResponseBookingStatus
            {
                RowID = bookingStatus.RowID,
                ColumnID = bookingStatus.ColumnID,
                SeatTypeID = (int)bookingStatus.SeatType,
                IsFree = bookingStatus.IsFree
            });

            return Ok(response);
        }

        [HttpPut]
        [SwaggerOperation(
            Summary = "Бронює місця"
        )]
        public async Task<ActionResult> CreateBooking(
            [FromBody] CreateBookingRequest incomingRequest,
            CancellationToken cancellationToken = default
        )
        {
            CreateBookingCommandInput commandInput = new CreateBookingCommandInput
            {
                SessionID = incomingRequest.SessionID,
                SeatsCoordinates = incomingRequest.SeatsCoordinates.Select(rawSeatCoordinates => new CreateBookingCommandInput.SeatCoordinates()
                {
                    RowID = rawSeatCoordinates.RowID,
                    ColumnID = rawSeatCoordinates.ColumnID
                }),
                ClientEmail = incomingRequest.ClientEmail,
                ClientPhone = incomingRequest.ClientPhone
            };

            await _mediator.Send(commandInput, cancellationToken);

            return Ok();
        }
    }
}