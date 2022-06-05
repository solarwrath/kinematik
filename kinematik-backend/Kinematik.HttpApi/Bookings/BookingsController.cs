using Kinematik.Application.Commands.Bookings;
using Kinematik.Application.Queries.Admin.Bookings;
using Kinematik.Application.Queries.Bookings;
using Kinematik.HttpApi.Bookings.CreateBooking;
using Kinematik.HttpApi.Bookings.GetBookingStatuses;
using Kinematik.HttpApi.Bookings.GetDetailedBookingStatuses;
using Kinematik.HttpApi.Bookings.ProcessPaymentPong;

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

        [HttpGet]
        [Route("{sessionID:int}/detailed")]
        [SwaggerOperation(
            Summary = "Повертає стан бронювань на зазначений сеанс"
        )]
        public async Task<ActionResult<GetDetailedBookingStatusesResponse>> GetDetailedBookingStatuses(
            [FromRoute] int sessionID,
            CancellationToken cancellationToken = default
        )
        {
            GetDetailedBookingStatusesResponse response = new GetDetailedBookingStatusesResponse();

            GetDetailedBookingStatusesQueryOutput queryOutput = await _mediator.Send(
                new GetDetailedBookingStatusesQueryInput
                {
                    SessionID = sessionID
                },
                cancellationToken
            );

            response.DetailedBookingStatuses = queryOutput.DetailedBookingStatuses.Select(bookingStatus => new GetDetailedBookingStatusesResponseBookingStatus
            {
                RowID = bookingStatus.RowID,
                ColumnID = bookingStatus.ColumnID,
                SeatTypeID = (int)bookingStatus.SeatType,
                SeatAvailabilityStatusID = (int)bookingStatus.SeatAvailabilityStatus,
                BookingOrderID = bookingStatus.BookingOrderID,
                BookedClientEmail = bookingStatus.BookedClientEmail,
                BookedClientPhone = bookingStatus.BookedClientPhone
            });

            return Ok(response);
        }

        [HttpPut]
        [SwaggerOperation(
            Summary = "Бронює місця"
        )]
        public async Task<ActionResult<CreateBookingResponse>> CreateBooking(
            [FromBody] CreateBookingRequest incomingRequest,
            CancellationToken cancellationToken = default
        )
        {
            CreateBookingResponse response = new CreateBookingResponse();

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

            CreateBookingCommandOutput commandOutput = await _mediator.Send(commandInput, cancellationToken);

            response.CheckoutRequestData = commandOutput.CheckoutRequestData;
            response.CheckoutRequestSignature = commandOutput.CheckoutRequestSignature;

            return Ok(response);
        }

        [HttpPost]
        [Route("liqpay-callback")]
        [SwaggerOperation(
            Summary = "LiqPay-коллбек"
        )]
        public async Task<ActionResult> ProcessLiqPayCallback(
            [FromForm] ProcessPaymentPongRequest request,
            CancellationToken cancellationToken = default
        )
        {
            ProcessLiqPayCallbackCommandInput commandInput = new ProcessLiqPayCallbackCommandInput
            {
                Data = request.Data,
                Signature = request.Signature
            };

            await _mediator.Send(commandInput, cancellationToken);
            
            return Ok();
        }
    }
}