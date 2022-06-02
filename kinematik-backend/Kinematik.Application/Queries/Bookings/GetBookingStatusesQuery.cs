using Kinematik.Domain.Entities;
using Kinematik.EntityFramework;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Kinematik.Application.Queries.Bookings
{
    public class GetBookingStatusesQueryInput : IRequest<GetBookingStatusesQueryOutput>
    {
        public int SessionID { get; set; }
    }

    public class GetBookingStatusesQueryHandler : IRequestHandler<GetBookingStatusesQueryInput, GetBookingStatusesQueryOutput>
    {
        private readonly KinematikDbContext _dbContext;

        public GetBookingStatusesQueryHandler(
            KinematikDbContext dbContext
        )
        {
            _dbContext = dbContext;
        }

        public async Task<GetBookingStatusesQueryOutput> Handle(GetBookingStatusesQueryInput request, CancellationToken cancellationToken)
        {
            GetBookingStatusesQueryOutput output = new GetBookingStatusesQueryOutput();

            output.BookingStatuses = await _dbContext.Sessions
                .Where(session => session.ID == request.SessionID)
                .Select(session => session.Hall)
                .SelectMany(hall => hall.LayoutItems)
                .Select(hallLayoutItem => new GetBookingStatusesQueryOutput.BookingStatus
                {
                    RowID = hallLayoutItem.RowID,
                    ColumnID = hallLayoutItem.ColumnID,
                    SeatType = hallLayoutItem.SeatType,
                    IsFree = !_dbContext.BookedSeats
                        .Any(bookedSeat => 
                            bookedSeat.SessionID == request.SessionID
                            && bookedSeat.RowID == hallLayoutItem.RowID
                            && bookedSeat.ColumnID == hallLayoutItem.ColumnID
                        )
                })
                .ToArrayAsync(cancellationToken);

            return output;
        }
    }

    public class GetBookingStatusesQueryOutput
    {
        public IEnumerable<BookingStatus> BookingStatuses { get; set; }

        public class BookingStatus
        {
            public int RowID { get; set; }
            public int ColumnID { get; set; }
            public SeatType SeatType { get; set; }
            public bool IsFree { get; set; }
        }
    }
}