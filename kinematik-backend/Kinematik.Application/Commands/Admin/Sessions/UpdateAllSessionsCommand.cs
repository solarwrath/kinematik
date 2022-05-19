using Kinematik.Domain.Entities;
using Kinematik.EntityFramework;

using MediatR;

namespace Kinematik.Application.Commands.Admin.Halls
{
    public class UpdateAllSessionsCommandInput : IRequest
    {
        public IEnumerable<UpdateAllSessionsCommandInputSession> UpdatedSessions { get; set; }
    }

    public class UpdateAllSessionsCommandInputSession
    {
        public int? ID { get; set; }
        public int FilmID { get; set; }
        public int? HallID { get; set; }
        public DateTime StartAt { get; set; }
    }

    public class UpdateAllSessionsCommandHandler : IRequestHandler<UpdateAllSessionsCommandInput>
    {
        private readonly KinematikDbContext _dbContext;

        public UpdateAllSessionsCommandHandler(
            KinematikDbContext dbContext
        )
        {
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(UpdateAllSessionsCommandInput input, CancellationToken cancellationToken)
        {
            _dbContext.Sessions.RemoveRange(_dbContext.Sessions);

            IEnumerable<Session> updatedSessions = input.UpdatedSessions
                .Select(addedSession =>
                {
                    Session session = new Session
                    {
                        FilmID = addedSession.FilmID,
                        HallID = addedSession.HallID,
                        StartAt = addedSession.StartAt
                    };

                    if (addedSession.ID.HasValue)
                    {
                        session.ID = addedSession.ID.Value;
                    }

                    return session;
                });
            _dbContext.Sessions.AddRange(updatedSessions);

            /*
            IEnumerable<Session> previousSessions = _dbContext.Sessions;
            IEnumerable<int> previousSessionIDs = _dbContext.Sessions.Select(previousSession => previousSession.ID);

            IEnumerable<UpdateAllSessionsCommandInputSession> mappedStillExistingSessions = input.UpdatedSessions.Where(updatedSession => updatedSession.ID.HasValue);
            IEnumerable<int> stillExistingSessionIDs = mappedStillExistingSessions.Select(nowExistingSession => nowExistingSession.ID.Value);

            IEnumerable<int> deletedSessionIDs = previousSessionIDs.Except(stillExistingSessionIDs);
            IEnumerable<Session> deletedSessions = previousSessions.Where(previousSession => deletedSessionIDs.Contains(previousSession.ID));
            _dbContext.Sessions.RemoveRange(deletedSessions);

            Dictionary<int, Session> stillExistingSessions = await _dbContext.Sessions.ToDictionaryAsync(stillExistingSession => stillExistingSession.ID);
            foreach (UpdateAllSessionsCommandInputSession mappedStillExistingSession in mappedStillExistingSessions)
            {
                Session correspondingSession = stillExistingSessions[mappedStillExistingSession.ID.Value];
                correspondingSession.HallID = mappedStillExistingSession.HallID;
                correspondingSession.StartAt = mappedStillExistingSession.StartAt;
            }

            IEnumerable<Session> addedSessions = input.UpdatedSessions
                .Where(nowExistingSession => !nowExistingSession.ID.HasValue)
                .Select(addedSession => new Session
                {
                    FilmID = addedSession.FilmID,
                    HallID = addedSession.HallID,
                    StartAt = addedSession.StartAt
                });
            await _dbContext.Sessions.AddRangeAsync(addedSessions, cancellationToken);
            */
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}