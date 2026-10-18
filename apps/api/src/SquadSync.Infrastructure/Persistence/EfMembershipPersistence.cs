using Microsoft.EntityFrameworkCore;
using Npgsql;
using SquadSync.Application.Memberships;
using SquadSync.Domain;

namespace SquadSync.Infrastructure.Persistence;

public sealed class EfMembershipPersistence(SquadSyncDbContext dbContext) : IMembershipPersistence
{
    public Task<MembershipDetails?> GetByIdAsync(
        Guid teamId, Guid membershipId, CancellationToken cancellationToken = default)
        => (from membership in dbContext.TeamMemberships.AsNoTracking()
            join person in dbContext.People.AsNoTracking() on membership.PersonId equals person.Id
            where membership.TeamId == teamId && membership.Id == membershipId
            select new MembershipDetails(membership.Id, membership.TeamId,
                new MembershipPerson(person.Id, person.FirstName, person.LastName), membership.TeamRole))
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<MembershipDetails>> GetByTeamIdAsync(
        Guid teamId, CancellationToken cancellationToken = default)
        => await (from membership in dbContext.TeamMemberships.AsNoTracking()
                  join person in dbContext.People.AsNoTracking() on membership.PersonId equals person.Id
                  where membership.TeamId == teamId
                  orderby person.LastName, person.FirstName, membership.Id
                  select new MembershipDetails(membership.Id, membership.TeamId,
                      new MembershipPerson(person.Id, person.FirstName, person.LastName), membership.TeamRole))
            .ToListAsync(cancellationToken);

    public async Task AddAsync(TeamMembership membership, CancellationToken cancellationToken = default)
    {
        dbContext.TeamMemberships.Add(membership);
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            dbContext.Entry(membership).State = EntityState.Detached;
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: "IX_TeamMemberships_PersonId_TeamId"
        })
        {
            dbContext.Entry(membership).State = EntityState.Detached;
            throw new MembershipConflictException(exception);
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.ForeignKeyViolation,
            ConstraintName: "FK_TeamMemberships_People_PersonId" or "FK_TeamMemberships_Teams_TeamId"
        })
        {
            dbContext.Entry(membership).State = EntityState.Detached;
            throw new MembershipReferenceNotFoundException(exception);
        }
    }
}
