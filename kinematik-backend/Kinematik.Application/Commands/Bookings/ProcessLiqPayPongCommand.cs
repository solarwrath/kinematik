using System.Security.Cryptography;
using System.Text;

using Kinematik.Application.Notifications.TicketBooked;
using Kinematik.Application.Ports;
using Kinematik.Application.ThirdParty.LiqPayAPI;
using Kinematik.Domain.Entities;
using Kinematik.EntityFramework;

using MediatR;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace Kinematik.Application.Commands.Bookings
{
    public class ProcessLiqPayCallbackCommandInput : IRequest
    {
        public string Data { get; set; }
        public string Signature { get; set; }
    }

    public class ProcessLiqPayCallbackCommandHandler : IRequestHandler<ProcessLiqPayCallbackCommandInput>
    {
        private readonly KinematikDbContext _dbContext;
        private readonly LiqPayConfiguration _liqPayConfiguration;
        private readonly IMediator _mediator;

        public ProcessLiqPayCallbackCommandHandler(
            KinematikDbContext dbContext,
            IOptions<LiqPayConfiguration> liqPayConfiguration,
            IMediator mediator
        )
        {
            _dbContext = dbContext;
            _liqPayConfiguration = liqPayConfiguration.Value;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(ProcessLiqPayCallbackCommandInput input, CancellationToken cancellationToken)
        {
            string desiredRawSignature = _liqPayConfiguration.PrivateKey + input.Data + _liqPayConfiguration.PrivateKey;
            byte[] desiredHashedSignature = Array.Empty<byte>();
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                desiredHashedSignature = sha1.ComputeHash(Encoding.UTF8.GetBytes(desiredRawSignature));
            }
            string desiredEncodedSignature = Convert.ToBase64String(desiredHashedSignature);

            bool isRequestValid = string.Equals(input.Signature, desiredEncodedSignature);
            if (!isRequestValid)
            {
                throw new Exception("Not valid response!!");
            }

            string serializedDecodedData = Encoding.UTF8.GetString(Convert.FromBase64String(input.Data));
            LiqPayCallback callbackData = JsonConvert.DeserializeObject<LiqPayCallback>(serializedDecodedData);

            if (string.Equals(callbackData.status, "success"))
            {
                bool canParseBookingID = int.TryParse(callbackData.order_id, out int bookingID);
                if (!canParseBookingID)
                {
                    throw new Exception("Can't parse booking id");
                }

                Booking? correspondingBooking = await _dbContext.Bookings.FindAsync(new object[] { bookingID }, cancellationToken);
                correspondingBooking.IsPayedFor = true;
                await _dbContext.SaveChangesAsync(cancellationToken);

                _mediator.Publish(
                    new TicketBookedNotification
                    {
                        BookingID = bookingID
                    },
                    cancellationToken
                );
            }

            return Unit.Value;
        }
    }
}