using Microsoft.EntityFrameworkCore;
using Npgsql;
using SquadSync.Domain;
using SquadSync.Infrastructure.Persistence;
using Xunit;

namespace SquadSync.IntegrationTests;

public class UserTeamPersistenceTests
{
    [DatabaseFact]
    [Trait("Category", "Database")]
    public async Task UserTeamAndMembership_RoundTripAndRejectDuplicateThroughPostgres_WithoutLeavingRows()
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__SquadSync");
        Assert.False(string.IsNullOrWhiteSpace(connectionString),
            "Database tests require ConnectionStrings__SquadSync when SQUADSYNC_RUN_DATABASE_TESTS=1.");

        var options = new DbContextOptionsBuilder<SquadSyncDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        await using var dbContext = new SquadSyncDbContext(options);
        var user = new User(Guid.NewGuid(), "  Alex  ", "  Morgan  ");
        var team = new Team(Guid.NewGuid(), "  SquadSync Persistence Test  ");
        var membership = new TeamMembership(Guid.NewGuid(), user.Id, team.Id, TeamRole.AssistantCoach);
        var duplicateMembership = new TeamMembership(Guid.NewGuid(), user.Id, team.Id, TeamRole.Player);

        await using (var transaction = await dbContext.Database.BeginTransactionAsync())
        {
            dbContext.Users.Add(user);
            dbContext.Teams.Add(team);
            dbContext.TeamMemberships.Add(membership);
            Assert.Equal(3, await dbContext.SaveChangesAsync());
            dbContext.ChangeTracker.Clear();

            var persistedUser = await dbContext.Users.SingleAsync(candidate => candidate.Id == user.Id);
            var persistedTeam = await dbContext.Teams.SingleAsync(candidate => candidate.Id == team.Id);
            var persistedMembership = await dbContext.TeamMemberships.SingleAsync(candidate => candidate.Id == membership.Id);

            Assert.NotSame(user, persistedUser);
            Assert.Equal(user.Id, persistedUser.Id);
            Assert.Equal("Alex", persistedUser.FirstName);
            Assert.Equal("Morgan", persistedUser.LastName);
            Assert.NotSame(team, persistedTeam);
            Assert.Equal(team.Id, persistedTeam.Id);
            Assert.Equal("SquadSync Persistence Test", persistedTeam.Name);
            Assert.NotSame(membership, persistedMembership);
            Assert.Equal(membership.Id, persistedMembership.Id);
            Assert.Equal(user.Id, persistedMembership.UserId);
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
            Assert.Equal("IX_TeamMemberships_UserId_TeamId", postgresException.ConstraintName);

            // Do no further database work in this transaction after the constraint failure.
            await transaction.RollbackAsync();
        }

        dbContext.ChangeTracker.Clear();
        Assert.False(await dbContext.Users.AnyAsync(candidate => candidate.Id == user.Id));
        Assert.False(await dbContext.Teams.AnyAsync(candidate => candidate.Id == team.Id));
        Assert.False(await dbContext.TeamMemberships.AnyAsync(candidate =>
            candidate.Id == membership.Id || candidate.Id == duplicateMembership.Id ||
            (candidate.UserId == user.Id && candidate.TeamId == team.Id)));
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
