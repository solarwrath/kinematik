using Kinematik.Application.Commands.Admin.Halls;
using Kinematik.Application.Queries.Admin.Halls;
using Kinematik.Domain.Entities;
using Kinematik.HttpApi.Halls.CreateHall;
using Kinematik.HttpApi.Halls.GetHall;
using Kinematik.HttpApi.Halls.GetHallsList;
using Kinematik.HttpApi.Halls.UpdateHall;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

namespace Kinematik.HttpApi.Halls
{
    public class HallsController : HttpApiControllerBase
    {
        private readonly IMediator _mediator;

        public HallsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Повертає список зал"
        )]
        public async Task<ActionResult<GetHallsListResponse>> GetHallsList(CancellationToken cancellationToken = default)
        {
            GetHallsListQueryOutput queryOutput = await _mediator.Send(new GetHallsListQueryInput(), cancellationToken);

            GetHallsListResponse response = new GetHallsListResponse
            {
                Halls = queryOutput.Halls.Select(originalFilm => new GetHallsListResponseMappedHall
                {
                    ID = originalFilm.ID,
                    Title = originalFilm.Title
                })
            };

            return Ok(response);
        }

        [HttpGet]
        [Route("{hallID:int}")]
        [SwaggerOperation(
            Summary = "Повертає повну інформацію про залу"
        )]
        public async Task<ActionResult<GetHallResponse>> GetHall(
            [FromRoute] int hallID,
            CancellationToken cancellationToken = default
        )
        {
            GetHallResponse response = new GetHallResponse();

            GetHallQueryInput queryInput = new GetHallQueryInput
            {
                HallID = hallID
            };
            GetHallQueryOutput queryOutput = await _mediator.Send(queryInput, cancellationToken);

            response.Title = queryOutput.Title;
            response.LayoutItems = queryOutput.LayoutItems.Select(rawLayoutItem => new GetHallResponseLayoutItem
            {
                RowID = rawLayoutItem.RowID,
                ColumnID = rawLayoutItem.ColumnID,
                SeatTypeID = (int)rawLayoutItem.SeatType
            });

            return Ok(response);
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Створює залу"
        )]
        public async Task<ActionResult<int>> CreateHall(
            [FromBody] CreateHallRequest incomingRequest,
            CancellationToken cancellationToken = default
        )
        {
            CreateHallCommandInput commandInput = new CreateHallCommandInput
            {
                Title = incomingRequest.Title,
                LayoutItems = incomingRequest.LayoutItems.Select(rawLayoutItem => new CreateHallCommandInput.LayoutItem
                {
                    RowID = rawLayoutItem.RowID,
                    ColumnID = rawLayoutItem.ColumnID,
                    SeatType = (SeatType)rawLayoutItem.SeatTypeID
                })
            };

            int createdHallID = await _mediator.Send(commandInput, cancellationToken);

            return Ok(createdHallID);
        }

        [HttpPut]
        [Route("{hallID:int}")]
        [SwaggerOperation(
            Summary = "Редагує залу"
        )]
        public async Task<ActionResult<int>> UpdateHall(
            [FromRoute] int hallID,
            [FromBody] UpdateHallRequest incomingRequest,
            CancellationToken cancellationToken = default
        )
        {
            UpdateHallCommandInput commandInput = new UpdateHallCommandInput
            {
                HallID = hallID,
                UpdatedTitle = incomingRequest.Title,
                UpdatedLayoutItems = incomingRequest.LayoutItems.Select(rawLayoutItem => new UpdateHallCommandInput.LayoutItem
                {
                    RowID = rawLayoutItem.RowID,
                    ColumnID = rawLayoutItem.ColumnID,
                    SeatType = (SeatType)rawLayoutItem.SeatTypeID
                })
            };

            await _mediator.Send(commandInput, cancellationToken);

            return Ok();
        }

        [HttpDelete]
        [Route("{hallID:int}")]
        [SwaggerOperation(
            Summary = "Видаляє залу"
        )]
        public async Task<ActionResult> DeleteHall(
            [FromRoute] int hallID,
            CancellationToken cancellationToken = default
        )
        {
            DeleteHallCommandInput commandInput = new DeleteHallCommandInput
            {
                HallID = hallID
            };

            await _mediator.Send(commandInput, cancellationToken);

            return Ok();
        }
    }
}