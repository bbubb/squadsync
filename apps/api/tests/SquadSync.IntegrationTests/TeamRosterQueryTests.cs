using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SquadSync.Application.Rosters;
using SquadSync.Domain;
using SquadSync.Infrastructure.Persistence;
using Xunit;

namespace SquadSync.IntegrationTests;

public class TeamRosterQueryTests
{
    [Fact]
    public async Task NpgsqlQuery_TranslatesTeamAndPlayerFilterAndRosterIdOrdering()
    {
        var capture = new CaptureCommand();
        await using var context = new SquadSyncDbContext(new DbContextOptionsBuilder<SquadSyncDbContext>()
            .UseNpgsql("Host=localhost;Database=unused")
            .AddInterceptors(new SuppressConnection(), capture).Options);
        var teamId = Guid.NewGuid();
        using var source = new CancellationTokenSource();
        Assert.Empty(await new EfRosterPersistence(context).GetByTeamIdAsync(teamId, source.Token));
        Assert.Contains("FROM \"RosterEntries\" AS r", capture.Sql);
        Assert.Contains("INNER JOIN \"TeamMemberships\" AS t ON r.\"TeamMembershipId\" = t.\"Id\"", capture.Sql);
        Assert.Contains("INNER JOIN \"People\" AS p ON t.\"PersonId\" = p.\"Id\"", capture.Sql);
        Assert.Contains("WHERE t.\"TeamId\" = @", capture.Sql);
        Assert.Contains("AND t.\"TeamRole\" = 'Player'", capture.Sql);
        Assert.Contains("ORDER BY p.\"LastName\", p.\"FirstName\", r.\"Id\"", capture.Sql);
        Assert.DoesNotContain("WHERE r.\"RosterStatus\"", capture.Sql);
        Assert.Equal(teamId, Assert.Single(capture.Parameters));
        Assert.Equal(source.Token, capture.Token);
        Assert.Empty(context.ChangeTracker.Entries());
    }

    [DatabaseFact]
    [Trait("Category", "Database")]
    public async Task Collection_FiltersAndProjectsInNameThenRosterIdOrder_WithoutLeavingRows()
    {
        var connection = Environment.GetEnvironmentVariable("ConnectionStrings__SquadSync");
        Assert.False(string.IsNullOrWhiteSpace(connection), "Opt-in database tests require ConnectionStrings__SquadSync.");
        await using var context = new SquadSyncDbContext(new DbContextOptionsBuilder<SquadSyncDbContext>().UseNpgsql(connection).Options);
        var team = new Team(Guid.NewGuid(), "Roster List Test");
        var otherTeam = new Team(Guid.NewGuid(), "Other Roster List Test");
        var people = new[]
        {
            new Person(Guid.NewGuid(), "Chris", "Delta"),
            new Person(Guid.NewGuid(), "Chris", "Delta"),
            new Person(Guid.NewGuid(), "Alex", "Delta"),
            new Person(Guid.NewGuid(), "Aaron", "Echo"),
            new Person(Guid.NewGuid(), "First", "Alpha"),
            new Person(Guid.NewGuid(), "Unrostered", "Alpha"),
            new Person(Guid.NewGuid(), "Other", "Alpha")
        };
        var memberships = people.Select((person, index) => new TeamMembership(
            index == 0 ? Guid.Parse("a0000000-0000-0000-0000-000000000001") :
            index == 1 ? Guid.Parse("a0000000-0000-0000-0000-000000000002") : Guid.NewGuid(),
            person.Id, index == 6 ? otherTeam.Id : team.Id, index == 4 ? TeamRole.Coach : TeamRole.Player)).ToArray();
        // Deliberately reverse roster-ID and membership-ID ordering for the identical names.
        var entries = new[]
        {
            new RosterEntry(Guid.Parse("b0000000-0000-0000-0000-000000000002"), memberships[0].Id, "007", RosterStatus.Active),
            new RosterEntry(Guid.Parse("b0000000-0000-0000-0000-000000000001"), memberships[1].Id, null, RosterStatus.Reserved),
            new RosterEntry(Guid.NewGuid(), memberships[2].Id, "12", RosterStatus.Suspended),
            new RosterEntry(Guid.NewGuid(), memberships[3].Id, "9", RosterStatus.Active),
            // Persistence permits this structural inconsistency; the read must exclude it.
            new RosterEntry(Guid.NewGuid(), memberships[4].Id, null, RosterStatus.Active),
            new RosterEntry(Guid.NewGuid(), memberships[6].Id, null, RosterStatus.Active)
        };
        await using (var transaction = await context.Database.BeginTransactionAsync())
        {
            context.Teams.AddRange(team, otherTeam);
            context.People.AddRange(people);
            context.TeamMemberships.AddRange(memberships);
            context.RosterEntries.AddRange(entries);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();
            var adapter = new EfRosterPersistence(context);
            var result = await adapter.GetByTeamIdAsync(team.Id);
            var expectedOrder = new[] { 2, 1, 0, 3 };
            Assert.Equal(expectedOrder.Select(index => new RosterDetails(entries[index].Id, memberships[index].Id,
                new RosterPerson(people[index].Id, people[index].FirstName, people[index].LastName),
                entries[index].JerseyNumber, entries[index].RosterStatus)), result);
            Assert.Equal(entries[5].Id, Assert.Single(await adapter.GetByTeamIdAsync(otherTeam.Id)).Id);
            Assert.Empty(await adapter.GetByTeamIdAsync(Guid.NewGuid()));
            var membershipList = await new EfMembershipPersistence(context).GetByTeamIdAsync(team.Id);
            Assert.Contains(membershipList, membership => membership.Id == memberships[5].Id);
            Assert.Contains(membershipList, membership => membership.Id == memberships[4].Id);
            Assert.Empty(context.ChangeTracker.Entries());
            await transaction.RollbackAsync();
        }
        context.ChangeTracker.Clear();
        var personIds = people.Select(person => person.Id).ToArray();
        var entryIds = entries.Select(entry => entry.Id).ToArray();
        Assert.False(await context.People.AnyAsync(person => personIds.Contains(person.Id)));
        Assert.False(await context.Teams.AnyAsync(candidate => candidate.Id == team.Id || candidate.Id == otherTeam.Id));
        Assert.False(await context.TeamMemberships.AnyAsync(membership => membership.TeamId == team.Id || membership.TeamId == otherTeam.Id));
        Assert.False(await context.RosterEntries.AnyAsync(entry => entryIds.Contains(entry.Id)));
    }

    // Execute the real Npgsql translation without opening a socket or writing data.
    private sealed class SuppressConnection : DbConnectionInterceptor
    {
        public override ValueTask<InterceptionResult> ConnectionOpeningAsync(DbConnection connection,
            ConnectionEventData eventData, InterceptionResult result, CancellationToken cancellationToken = default)
            => ValueTask.FromResult(InterceptionResult.Suppress());
    }

    private sealed class CaptureCommand : DbCommandInterceptor
    {
        public string Sql { get; private set; } = "";
        public object?[] Parameters { get; private set; } = [];
        public CancellationToken Token { get; private set; }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command,
            CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            Sql = command.CommandText;
            Parameters = command.Parameters.Cast<DbParameter>().Select(parameter => parameter.Value).ToArray();
            Token = cancellationToken;
            return ValueTask.FromResult(InterceptionResult<DbDataReader>.SuppressWithResult(new DataTable().CreateDataReader()));
        }
    }
}
