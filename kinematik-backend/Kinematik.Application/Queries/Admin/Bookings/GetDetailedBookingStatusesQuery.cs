using Kinematik.Domain.ApplicationSideEnums;
using Kinematik.Domain.Entities;
using Kinematik.EntityFramework;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Kinematik.Application.Queries.Admin.Bookings
{
    public class GetDetailedBookingStatusesQueryInput : IRequest<GetDetailedBookingStatusesQueryOutput>
    {
        public int SessionID { get; set; }
    }

    public class GetDetailedBookingStatusesQueryHandler : IRequestHandler<GetDetailedBookingStatusesQueryInput, GetDetailedBookingStatusesQueryOutput>
    {
        private readonly KinematikDbContext _dbContext;

        public GetDetailedBookingStatusesQueryHandler(
            KinematikDbContext dbContext
        )
        {
            _dbContext = dbContext;
        }

        public async Task<GetDetailedBookingStatusesQueryOutput> Handle(GetDetailedBookingStatusesQueryInput request, CancellationToken cancellationToken)
        {
            GetDetailedBookingStatusesQueryOutput output = new GetDetailedBookingStatusesQueryOutput();

            IQueryable<Session> session = _dbContext.Sessions.Where(session => session.ID == request.SessionID);

            IQueryable<Hall?> hall = session.Select(session => session.Hall);
            IQueryable<HallLayoutItem> hallLayoutItems = hall.SelectMany(hall => hall.LayoutItems);

            IQueryable<Booking> bookings = session.SelectMany(session => session.Bookings);
            IQueryable<BookedSeat> bookedSeats = bookings.SelectMany(booking => booking.BookedSeats);

            output.DetailedBookingStatuses = await hallLayoutItems
                .Select(hallLayoutItem => new
                {
                    hallLayoutItem.RowID,
                    hallLayoutItem.ColumnID,
                    hallLayoutItem.SeatType,
                    BookingOrder = bookedSeats
                        .Where(bookedSeat =>
                            bookedSeat.RowID == hallLayoutItem.RowID
                            && bookedSeat.ColumnID == hallLayoutItem.ColumnID
                        )
                        .Select(bookedSeat => bookedSeat.Booking)
                        .Where(booking => booking.SessionID == request.SessionID)
                        .Select(booking => new
                        {
                            BookingOrderID = booking.ID,
                            BookedClientEmail = booking.ClientEmail,
                            BookedClientPhone = booking.ClientPhone,
                            booking.IsPayedFor
                        })
                        .SingleOrDefault()
                })
                .Select(hallLayoutItemWithBookingData => new GetDetailedBookingStatusesQueryOutput.DetailedBookingStatus
                {
                    RowID = hallLayoutItemWithBookingData.RowID,
                    ColumnID = hallLayoutItemWithBookingData.ColumnID,
                    SeatType = hallLayoutItemWithBookingData.SeatType,
                    SeatAvailabilityStatus = hallLayoutItemWithBookingData.BookingOrder == null
                        ? SeatAvailabilityStatus.FREE
                        : hallLayoutItemWithBookingData.BookingOrder.IsPayedFor
                            ? SeatAvailabilityStatus.PAYED_FOR
                            : SeatAvailabilityStatus.BOOKED,
                    BookingOrderID = hallLayoutItemWithBookingData.BookingOrder != null ? hallLayoutItemWithBookingData.BookingOrder.BookingOrderID : null,
                    BookedClientEmail = hallLayoutItemWithBookingData.BookingOrder != null ? hallLayoutItemWithBookingData.BookingOrder.BookedClientEmail : null,
                    BookedClientPhone = hallLayoutItemWithBookingData.BookingOrder != null ? hallLayoutItemWithBookingData.BookingOrder.BookedClientPhone : null,
                })
                .ToArrayAsync(cancellationToken);

            return output;
        }
    }

    public class GetDetailedBookingStatusesQueryOutput
    {
        public IEnumerable<DetailedBookingStatus> DetailedBookingStatuses { get; set; }

        public class DetailedBookingStatus
        {
            public int RowID { get; set; }
            public int ColumnID { get; set; }
            public SeatType SeatType { get; set; }
            public SeatAvailabilityStatus SeatAvailabilityStatus { get; set; }
            public int? BookingOrderID { get; set; }
            public string? BookedClientEmail { get; set; }
            public string? BookedClientPhone { get; set; }
        }
    }
}
