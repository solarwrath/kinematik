namespace Kinematik.Application.EmailTemplates.BookedTicketConfirmation
{
    public class BookedTicketConfirmationModel
    {
        public string Subject { get; set; }
        public string FilmTitle { get; set; }
        public int BookingID { get; set; }
        public string FormattedSessionStartDate { get; set; }
        public string HallTitle { get; set; }
        public IEnumerable<BookedSeat> BookedSeats { get; set; }
        public decimal TotalPrice => this.BookedSeats.Sum(bookedSeat => bookedSeat.Price);
        public string ClientEmail { get; set; }
        // public string FormattedPaymentDate { get; set; }
        // public string CustomSupportEmail { get; set; }

        public class BookedSeat
        {
            public int RowID { get; set; }
            public int ColumnID { get; set; }
            public string SeatType { get; set; }
            public decimal Price { get; set; }
        }
    }
}