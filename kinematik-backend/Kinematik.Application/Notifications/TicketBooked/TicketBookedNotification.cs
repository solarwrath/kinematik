using MediatR;

namespace Kinematik.Application.Notifications.TicketBooked
{
    public class TicketBookedNotification : INotification
    {
        public int BookingID { get; set; }
    }
}
