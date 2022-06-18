using System.Reflection;

using FluentEmail.Core;
using FluentEmail.Core.Models;

using Kinematik.Application.EmailTemplates.BookedTicketConfirmation;
using Kinematik.Domain.Entities;
using Kinematik.EntityFramework;

using MediatR;

using Microsoft.EntityFrameworkCore;

using QRCoder;

namespace Kinematik.Application.Commands.Bookings
{
    public class SendTicketBookedEmailCommandInput : IRequest
    {
        public int BookingID { get; set; }
    }

    public class SendTicketBookedEmailCommandHandler : IRequestHandler<SendTicketBookedEmailCommandInput>
    {
        private readonly KinematikDbContext _dbContext;
        private IFluentEmail _fluentEmail;

        public SendTicketBookedEmailCommandHandler(
            KinematikDbContext dbContext,
            IFluentEmail fluentEmail
        )
        {
            _dbContext = dbContext;
            _fluentEmail = fluentEmail;
        }

        // TODO outsource into db table
        private static readonly Dictionary<SeatType, Tuple<string, decimal>> SEAT_TYPE_DATA = new Dictionary<SeatType, Tuple<string, decimal>>
        {
            {SeatType.COMMON, new Tuple<string, decimal>("Звичайне", 65)},
            {SeatType.VIP, new Tuple<string, decimal>("VIP", 100)},
            {SeatType.COUCH, new Tuple<string, decimal>("Диван", 150)},
        };

        public async Task<Unit> Handle(SendTicketBookedEmailCommandInput input, CancellationToken cancellationToken)
        {
            var booking = await _dbContext.Bookings
                .Where(booking => booking.ID == input.BookingID)
                .Where(booking => booking.IsPayedFor)
                .Select(booking => new
                {
                    BookingID = booking.ID,
                    booking.ClientEmail,
                    booking.BookedAt,
                    FilmTitle = booking.Session.Film.Title,
                    HallTitle = booking.Session.Hall.Title,
                    BookedSeats = booking.BookedSeats.Select(bookedSeat => new
                    {
                        bookedSeat.RowID,
                        bookedSeat.ColumnID,
                        SeatType = booking.Session.Hall.LayoutItems
                            .Where(layoutItem =>
                                layoutItem.RowID == bookedSeat.RowID
                                && layoutItem.ColumnID == bookedSeat.ColumnID
                            )
                            .Select(layoutItem => layoutItem.SeatType)
                            .SingleOrDefault()
                    }),
                    SessionStartAt = booking.Session.StartAt
                })
                .SingleOrDefaultAsync(cancellationToken);

            string subject = $"Квиток на фільм \"{booking.FilmTitle}\"";

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(booking.BookingID.ToString(), QRCodeGenerator.ECCLevel.Q);

            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeImageBuffer = qrCode.GetGraphic(20);
            using (MemoryStream qrCodeImageStream = new MemoryStream(qrCodeImageBuffer))
            {
                await _fluentEmail.To(booking.ClientEmail)
                    .Subject(subject)
                    .UsingTemplateFromEmbedded(
                        "Kinematik.Application.EmailTemplates.BookedTicketConfirmation.BookedTicketConfirmation.cshtml",
                        new BookedTicketConfirmationModel
                        {
                            Subject = subject,
                            FilmTitle = booking.FilmTitle,
                            BookingID = booking.BookingID,
                            FormattedSessionStartDate = booking.SessionStartAt.ToString("ddd, dd MMMM HH:mm"),
                            HallTitle = booking.HallTitle,
                            BookedSeats = booking.BookedSeats.Select(bookedSeat => new BookedTicketConfirmationModel.BookedSeat
                            {
                                RowID = bookedSeat.RowID,
                                ColumnID = bookedSeat.ColumnID,
                                SeatType = SEAT_TYPE_DATA[bookedSeat.SeatType].Item1,
                                Price = SEAT_TYPE_DATA[bookedSeat.SeatType].Item2
                            }),
                            ClientEmail = booking.ClientEmail,
                            // TODO Mb save this??
                            // FormattedPaymentDate = booking.SessionStartAt.ToString("dddd, dd MMMM HH:mm"),
                            // TODO Get in config
                            // CustomSupportEmail = "test@kinematik.org"
                        },
                        Assembly.GetExecutingAssembly()
                    )
                    .Attach(new Attachment()
                    {
                        ContentId = "qr-code",
                        Data = qrCodeImageStream,
                        ContentType = "image/png",
                        Filename = $"kinematik-ticket-qr-code-{booking.BookingID}.png",
                        IsInline = true
                    })
                    .HighPriority()
                    .SendAsync(cancellationToken);
            }

            return Unit.Value;
        }
    }
}