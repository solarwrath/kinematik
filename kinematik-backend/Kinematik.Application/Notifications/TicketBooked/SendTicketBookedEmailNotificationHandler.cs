using Kinematik.Application.Commands.Bookings;

using MediatR;

namespace Kinematik.Application.Notifications.TicketBooked
{
    public class SendTicketBookedEmailNotificationHandler : INotificationHandler<TicketBookedNotification>
    {
        private readonly IMediator _mediator;

        public SendTicketBookedEmailNotificationHandler(
            IMediator mediator
        )
        {
            _mediator = mediator;
        }

        public async Task Handle(TicketBookedNotification notification, CancellationToken cancellationToken)
        {
            await _mediator.Send(
                new SendTicketBookedEmailCommandInput
                {
                    BookingID = notification.BookingID
                },
                cancellationToken
            );
        }
    }
}