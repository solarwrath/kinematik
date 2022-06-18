using Kinematik.EntityFramework;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Kinematik.Application.Queries.Bookings
{
    public class CheckBookingTicketQueryInput : IRequest<CheckBookingTicketQueryOutput>
    {
        public int BookingID { get; set; }
        public int HallID { get; set; }
    }

    public class CheckBookingTicketQueryHandler : IRequestHandler<CheckBookingTicketQueryInput, CheckBookingTicketQueryOutput>
    {
        private readonly KinematikDbContext _dbContext;

        public CheckBookingTicketQueryHandler(
            KinematikDbContext dbContext
        )
        {
            _dbContext = dbContext;
        }

        public async Task<CheckBookingTicketQueryOutput> Handle(CheckBookingTicketQueryInput request, CancellationToken cancellationToken)
        {
            CheckBookingTicketQueryOutput output = new CheckBookingTicketQueryOutput();

            var checkedData = await _dbContext.Bookings
                .Where(booking => booking.ID == request.BookingID)
                .Select(booking => new
                {
                    booking.IsPayedFor,
                    booking.Session.HallID
                })
                .SingleOrDefaultAsync(cancellationToken);
            if (checkedData == null)
            {
                output.Error = CheckBookingTicketQueryOutput.ErrorType.NOT_EXISTING;
                return output;
            }

            if (!checkedData.IsPayedFor)
            {
                output.Error = CheckBookingTicketQueryOutput.ErrorType.NOT_PAYED_FOR;
                return output;
            }

            if (checkedData.HallID != request.HallID)
            {
                output.Error = CheckBookingTicketQueryOutput.ErrorType.WRONG_HALL;
                return output;
            }

            return output;
        }
    }

    public class CheckBookingTicketQueryOutput
    {
        public ErrorType? Error { get; set; }

        public enum ErrorType
        {
            NOT_EXISTING = 1,
            NOT_PAYED_FOR = 2,
            WRONG_HALL = 3
        }
    }
}