using Kinematik.Domain.Entities;
using Kinematik.EntityFramework;

using MediatR;

namespace Kinematik.Application.Commands.Bookings
{
    public class CreateBookingCommandInput : IRequest
    {
        public int SessionID { get; set; }
        public IEnumerable<SeatCoordinates> SeatsCoordinates { get; set; }
        public string ClientEmail { get; set; }
        public string ClientPhone { get; set; }

        public class SeatCoordinates
        {
            public int RowID { get; set; }
            public int ColumnID { get; set; }
        }
    }

    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommandInput>
    {
        private readonly KinematikDbContext _dbContext;

        public CreateBookingCommandHandler(
            KinematikDbContext dbContext
        )
        {
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(CreateBookingCommandInput input, CancellationToken cancellationToken)
        {
            Booking newBooking = new Booking
            {
                SessionID = input.SessionID,
                ClientEmail = input.ClientEmail,
                ClientPhone = input.ClientPhone,
            };
            newBooking.BookedSeats = input.SeatsCoordinates
                .Select(seatCoordinates => new BookedSeat
                {
                    Booking = newBooking,
                    SessionID = input.SessionID,
                    RowID = seatCoordinates.RowID,
                    ColumnID = seatCoordinates.ColumnID
                })
                .ToArray();

            _dbContext.Add(newBooking);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}