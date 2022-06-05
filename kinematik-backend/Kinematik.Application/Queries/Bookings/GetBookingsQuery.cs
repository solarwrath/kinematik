using Kinematik.Domain.Entities;
using Kinematik.EntityFramework;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Kinematik.Application.Queries.Bookings
{
    public class GetBookingsQueryInput : IRequest<GetBookingsQueryOutput>
    {
    }

    public class GetBookingsQueryHandler : IRequestHandler<GetBookingsQueryInput, GetBookingsQueryOutput>
    {
        private readonly KinematikDbContext _dbContext;

        public GetBookingsQueryHandler(
            KinematikDbContext dbContext
        )
        {
            _dbContext = dbContext;
        }

        public async Task<GetBookingsQueryOutput> Handle(GetBookingsQueryInput request, CancellationToken cancellationToken)
        {
            GetBookingsQueryOutput output = new GetBookingsQueryOutput();

            return output;
        }
    }

    public class GetBookingsQueryOutput
    {
        public int ID { get; set; }
        public int SessionID { get; set; }
        public Session Session { get; set; }
        public bool IsPayedFor { get; set; }
        public string ClientPhone { get; set; }
        public string ClientEmail { get; set; }
        public DateTime BookedAt { get; set; }

        public virtual ICollection<BookedSeat> BookedSeats { get; set; }

    }
}