using Microsoft.EntityFrameworkCore;
using SquadSync.Domain;
using SquadSync.Infrastructure.Persistence;
using Xunit;

namespace SquadSync.IntegrationTests;

public class UserTeamPersistenceTests
{
    [DatabaseFact]
    [Trait("Category", "Database")]
    public async Task UserAndTeam_RoundTripThroughPostgres_WithoutLeavingRows()
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

        await using (var transaction = await dbContext.Database.BeginTransactionAsync())
        {
            dbContext.Users.Add(user);
            dbContext.Teams.Add(team);
            Assert.Equal(2, await dbContext.SaveChangesAsync());
            dbContext.ChangeTracker.Clear();

            var persistedUser = await dbContext.Users.SingleAsync(candidate => candidate.Id == user.Id);
            var persistedTeam = await dbContext.Teams.SingleAsync(candidate => candidate.Id == team.Id);

            Assert.NotSame(user, persistedUser);
            Assert.Equal(user.Id, persistedUser.Id);
            Assert.Equal("Alex", persistedUser.FirstName);
            Assert.Equal("Morgan", persistedUser.LastName);
            Assert.NotSame(team, persistedTeam);
            Assert.Equal(team.Id, persistedTeam.Id);
            Assert.Equal("SquadSync Persistence Test", persistedTeam.Name);

            await transaction.RollbackAsync();
        }

        dbContext.ChangeTracker.Clear();
        Assert.False(await dbContext.Users.AnyAsync(candidate => candidate.Id == user.Id));
        Assert.False(await dbContext.Teams.AnyAsync(candidate => candidate.Id == team.Id));
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
