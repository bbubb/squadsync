using Microsoft.EntityFrameworkCore;
using Npgsql;
using SquadSync.Application.Rosters;
using SquadSync.Domain;

namespace SquadSync.Infrastructure.Persistence;

public sealed class EfRosterPersistence(SquadSyncDbContext dbContext) : IRosterPersistence, IRosterQueries
{
    public async Task<IReadOnlyList<RosterDetails>> GetByTeamIdAsync(Guid teamId, CancellationToken cancellationToken = default)
        => await (from entry in dbContext.RosterEntries.AsNoTracking()
            join membership in dbContext.TeamMemberships.AsNoTracking() on entry.TeamMembershipId equals membership.Id
            join person in dbContext.People.AsNoTracking() on membership.PersonId equals person.Id
            where membership.TeamId == teamId && membership.TeamRole == TeamRole.Player
            orderby person.LastName, person.FirstName, entry.Id
            select new RosterDetails(entry.Id, membership.Id,
                new RosterPerson(person.Id, person.FirstName, person.LastName), entry.JerseyNumber, entry.RosterStatus))
            .ToListAsync(cancellationToken);

    public Task<RosterDetails?> GetByIdAsync(Guid teamId, Guid rosterEntryId, CancellationToken cancellationToken = default)
        => (from entry in dbContext.RosterEntries.AsNoTracking()
            join membership in dbContext.TeamMemberships.AsNoTracking() on entry.TeamMembershipId equals membership.Id
            join person in dbContext.People.AsNoTracking() on membership.PersonId equals person.Id
            where entry.Id == rosterEntryId && membership.TeamId == teamId && membership.TeamRole == TeamRole.Player
            select new RosterDetails(entry.Id, membership.Id,
                new RosterPerson(person.Id, person.FirstName, person.LastName), entry.JerseyNumber, entry.RosterStatus))
            .SingleOrDefaultAsync(cancellationToken);

    public Task<TeamMembership?> GetMembershipAsync(Guid membershipId, CancellationToken cancellationToken = default)
        => dbContext.TeamMemberships.AsNoTracking()
            .SingleOrDefaultAsync(membership => membership.Id == membershipId, cancellationToken);

    public async Task AddEntryAsync(RosterEntry entry, CancellationToken cancellationToken = default)
    {
        dbContext.RosterEntries.Add(entry);
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            // Keep later attempts on this scoped adapter subject to database constraints,
            // including another request with the same entry ID.
            dbContext.Entry(entry).State = EntityState.Detached;
        }
        catch (DbUpdateException exception) when (
            exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation,
                ConstraintName: "IX_RosterEntries_TeamMembershipId" or "PK_RosterEntries"
            })
        {
            dbContext.Entry(entry).State = EntityState.Detached;
            throw new RosterEntryConflictException(entry.Id, entry.TeamMembershipId, exception);
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.ForeignKeyViolation,
            ConstraintName: "FK_RosterEntries_TeamMemberships_TeamMembershipId"
        })
        {
            dbContext.Entry(entry).State = EntityState.Detached;
            throw new RosterMembershipNotFoundException(entry.TeamMembershipId, exception);
        }
    }
}
