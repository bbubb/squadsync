using Microsoft.EntityFrameworkCore;
using Npgsql;
using SquadSync.Domain;
using SquadSync.Infrastructure.Persistence;
using Xunit;

namespace SquadSync.IntegrationTests;

public class PersonTeamPersistenceTests
{
    [DatabaseFact]
    [Trait("Category", "Database")]
    public async Task PersonTeamAndMembership_RoundTripAndRejectDuplicateThroughPostgres_WithoutLeavingRows()
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__SquadSync");
        Assert.False(string.IsNullOrWhiteSpace(connectionString),
            "Database tests require ConnectionStrings__SquadSync when SQUADSYNC_RUN_DATABASE_TESTS=1.");

        var options = new DbContextOptionsBuilder<SquadSyncDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        await using var dbContext = new SquadSyncDbContext(options);
        var person = new Person(Guid.NewGuid(), "  Alex  ", "  Morgan  ");
        var team = new Team(Guid.NewGuid(), "  SquadSync Persistence Test  ");
        var membership = new TeamMembership(Guid.NewGuid(), person.Id, team.Id, TeamRole.AssistantCoach);
        var duplicateMembership = new TeamMembership(Guid.NewGuid(), person.Id, team.Id, TeamRole.Player);

        await using (var transaction = await dbContext.Database.BeginTransactionAsync())
        {
            dbContext.People.Add(person);
            dbContext.Teams.Add(team);
            dbContext.TeamMemberships.Add(membership);
            Assert.Equal(3, await dbContext.SaveChangesAsync());
            dbContext.ChangeTracker.Clear();

            var persistedPerson = await dbContext.People.SingleAsync(candidate => candidate.Id == person.Id);
            var persistedTeam = await dbContext.Teams.SingleAsync(candidate => candidate.Id == team.Id);
            var persistedMembership = await dbContext.TeamMemberships.SingleAsync(candidate => candidate.Id == membership.Id);

            Assert.NotSame(person, persistedPerson);
            Assert.Equal(person.Id, persistedPerson.Id);
            Assert.Equal("Alex", persistedPerson.FirstName);
            Assert.Equal("Morgan", persistedPerson.LastName);
            Assert.NotSame(team, persistedTeam);
            Assert.Equal(team.Id, persistedTeam.Id);
            Assert.Equal("SquadSync Persistence Test", persistedTeam.Name);
            Assert.NotSame(membership, persistedMembership);
            Assert.Equal(membership.Id, persistedMembership.Id);
            Assert.Equal(person.Id, persistedMembership.PersonId);
            Assert.Equal(team.Id, persistedMembership.TeamId);
            Assert.Equal(TeamRole.AssistantCoach, persistedMembership.TeamRole);

            var storedRole = await dbContext.Database.SqlQuery<string>(
                $"""SELECT "TeamRole" AS "Value" FROM "TeamMemberships" WHERE "Id" = {membership.Id}""")
                .SingleAsync();
            Assert.Equal("AssistantCoach", storedRole);

            dbContext.TeamMemberships.Add(duplicateMembership);
            var exception = await Assert.ThrowsAsync<DbUpdateException>(() => dbContext.SaveChangesAsync());
            var postgresException = Assert.IsType<PostgresException>(exception.InnerException);
            Assert.Equal(PostgresErrorCodes.UniqueViolation, postgresException.SqlState);
            Assert.Equal("IX_TeamMemberships_PersonId_TeamId", postgresException.ConstraintName);

            // Do no further database work in this transaction after the constraint failure.
            await transaction.RollbackAsync();
        }

        dbContext.ChangeTracker.Clear();
        Assert.False(await dbContext.People.AnyAsync(candidate => candidate.Id == person.Id));
        Assert.False(await dbContext.Teams.AnyAsync(candidate => candidate.Id == team.Id));
        Assert.False(await dbContext.TeamMemberships.AnyAsync(candidate =>
            candidate.Id == membership.Id || candidate.Id == duplicateMembership.Id ||
            (candidate.PersonId == person.Id && candidate.TeamId == team.Id)));
    }
}

public sealed class DatabaseFactAttribute : FactAttribute
{
    public DatabaseFactAttribute()
    {
        if (Environment.GetEnvironmentVariable("SQUADSYNC_RUN_DATABASE_TESTS") != "1")
        {
            Skip = "Set SQUADSYNC_RUN_DATABASE_TESTS=1 to run against local PostgreSQL.";
        }
    }
}
