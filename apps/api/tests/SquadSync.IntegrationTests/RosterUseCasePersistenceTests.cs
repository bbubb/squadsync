using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SquadSync.Application.Rosters;
using SquadSync.Domain;
using SquadSync.Infrastructure.Persistence;
using Xunit;

namespace SquadSync.IntegrationTests;

public class RosterUseCasePersistenceTests
{
    [DatabaseFact]
    [Trait("Category", "Database")]
    public async Task UseCase_PersistsEntryAndTranslatesMembershipAndIdConflicts_WithoutLeavingRows()
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__SquadSync");
        Assert.False(string.IsNullOrWhiteSpace(connectionString),
            "Database tests require ConnectionStrings__SquadSync when SQUADSYNC_RUN_DATABASE_TESTS=1.");
        var services = new ServiceCollection().AddSquadSyncPersistence(connectionString);
        await using var provider = services.BuildServiceProvider();

        // Each conflict gets its own transaction; rollback is the next database operation after failure.
        foreach (var duplicateId in new[] { false, true })
        {
            await using var scope = provider.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SquadSyncDbContext>();
            var persistence = scope.ServiceProvider.GetRequiredService<IRosterPersistence>();
            var useCase = new AddPlayerToRoster(persistence);
            var person = new Person(Guid.NewGuid(), "Roster", "UseCase");
            var otherPerson = new Person(Guid.NewGuid(), "Other", "Player");
            var team = new Team(Guid.NewGuid(), "Application Roster Validation");
            var membership = new TeamMembership(Guid.NewGuid(), person.Id, team.Id, TeamRole.Player);
            var otherMembership = new TeamMembership(Guid.NewGuid(), otherPerson.Id, team.Id, TeamRole.Player);
            var entryId = Guid.NewGuid();
            var conflictId = duplicateId ? entryId : Guid.NewGuid();
            var conflictMembershipId = duplicateId ? otherMembership.Id : membership.Id;

            await using (var transaction = await dbContext.Database.BeginTransactionAsync())
            {
                dbContext.People.AddRange(person, otherPerson);
                dbContext.Teams.Add(team);
                dbContext.TeamMemberships.AddRange(membership, otherMembership);
                await dbContext.SaveChangesAsync();
                dbContext.ChangeTracker.Clear();

                Assert.Null(await persistence.GetMembershipAsync(Guid.NewGuid()));
                var entry = await useCase.ExecuteAsync(entryId, membership.Id, "007", RosterStatus.Reserved);
                var persisted = await dbContext.RosterEntries.AsNoTracking().SingleAsync(candidate => candidate.Id == entry.Id);
                Assert.Equal(membership.Id, persisted.TeamMembershipId);
                Assert.Equal("007", persisted.JerseyNumber);
                Assert.Equal(RosterStatus.Reserved, persisted.RosterStatus);

                var exception = await Assert.ThrowsAsync<RosterEntryConflictException>(() =>
                    useCase.ExecuteAsync(conflictId, conflictMembershipId, null, RosterStatus.Active));
                Assert.Equal(conflictId, exception.RosterEntryId);
                Assert.Equal(conflictMembershipId, exception.TeamMembershipId);
                Assert.IsType<DbUpdateException>(exception.InnerException);
                await transaction.RollbackAsync();
            }

            dbContext.ChangeTracker.Clear();
            Assert.False(await dbContext.People.AnyAsync(candidate => candidate.Id == person.Id || candidate.Id == otherPerson.Id));
            Assert.False(await dbContext.Teams.AnyAsync(candidate => candidate.Id == team.Id));
            Assert.False(await dbContext.TeamMemberships.AnyAsync(candidate => candidate.TeamId == team.Id));
            Assert.False(await dbContext.RosterEntries.AnyAsync(candidate => candidate.Id == entryId || candidate.Id == conflictId));
        }
    }
}
