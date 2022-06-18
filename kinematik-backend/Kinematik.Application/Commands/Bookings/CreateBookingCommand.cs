using System.Security.Cryptography;
using System.Text;

using Kinematik.Application.Ports;
using Kinematik.Application.ThirdParty.LiqPayAPI;
using Kinematik.Domain.Entities;
using Kinematik.EntityFramework;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace Kinematik.Application.Commands.Bookings
{
    public class CreateBookingCommandInput : IRequest<CreateBookingCommandOutput>
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

    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommandInput, CreateBookingCommandOutput>
    {
        private readonly KinematikDbContext _dbContext;
        private readonly LiqPayConfiguration _liqPayConfiguration;

        public CreateBookingCommandHandler(
            KinematikDbContext dbContext,
            IOptions<LiqPayConfiguration> liqPayConfiguration
        )
        {
            _dbContext = dbContext;
            _liqPayConfiguration = liqPayConfiguration.Value;
        }

        // TODO outsource into db table
        private static readonly Dictionary<SeatType, decimal> PRICES_PER_SEAT_TYPE = new Dictionary<SeatType, decimal>
        {
            {SeatType.EMPTY, 0},
            {SeatType.COMMON, 65},
            {SeatType.VIP, 100},
            {SeatType.COUCH, 150}
        };

        public async Task<CreateBookingCommandOutput> Handle(CreateBookingCommandInput input, CancellationToken cancellationToken)
        {
            CreateBookingCommandOutput output = new CreateBookingCommandOutput();

            Booking newBooking = new Booking
            {
                SessionID = input.SessionID,
                ClientEmail = input.ClientEmail,
                ClientPhone = input.ClientPhone,
                BookedAt = DateTime.UtcNow
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

            Session? correspondingSession = await _dbContext.Sessions.FindAsync(new object[] { newBooking.SessionID }, cancellationToken);
            
            IEnumerable<HallLayoutItem> allSeatsInHall = await _dbContext.HallLayoutItems
                .Where(hallLayoutItem => hallLayoutItem.HallID == correspondingSession.HallID)
                .ToArrayAsync(cancellationToken);

            IEnumerable<HallLayoutItem> bookedSeats = allSeatsInHall.Where(
                seat => newBooking.BookedSeats.Any(bookedSeat =>
                    bookedSeat.RowID == seat.RowID
                    && bookedSeat.ColumnID == seat.ColumnID
                )
            );
            decimal totalPrice = bookedSeats.Sum(bookedSeat => PRICES_PER_SEAT_TYPE[bookedSeat.SeatType]);
            
            string[] bookedSeatsCoordinatesLabels =
                newBooking.BookedSeats
                    .Select(bookedSeat => $"{bookedSeat.RowID}/{bookedSeat.ColumnID}")
                    .ToArray();
            LiqPayCheckoutRequest checkoutCheckoutRequest = new LiqPayCheckoutRequest
            {
                version = 3,
                public_key = _liqPayConfiguration.PublicKey,
                private_key = _liqPayConfiguration.PrivateKey,
                action = "pay",
                amount = totalPrice,
                currency = "UAH",
                description = $"Замовлення №{newBooking.ID}. Місця: {string.Join(", ", bookedSeatsCoordinatesLabels)}.",
                order_id = newBooking.ID.ToString(),
                language = "ua",
                // TODO Deal with this
                server_url = "https://9f23-81-24-208-241.eu.ngrok.io/api/bookings/liqpay-callback"
            };

            string checkoutRequestJson = JsonConvert.SerializeObject(checkoutCheckoutRequest);
            string data = Convert.ToBase64String(Encoding.UTF8.GetBytes(checkoutRequestJson));
            output.CheckoutRequestData = data;

            string rawSignature = _liqPayConfiguration.PrivateKey + data + _liqPayConfiguration.PrivateKey;
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                byte[] hashedSignature = sha1.ComputeHash(Encoding.UTF8.GetBytes(rawSignature));
                string encodedSignature = Convert.ToBase64String(hashedSignature);

                output.CheckoutRequestSignature = encodedSignature;
            }

            return output;
        }
    }

    public class CreateBookingCommandOutput
    {
        public string CheckoutRequestData { get; set; }
        public string CheckoutRequestSignature { get; set; }
    }
}