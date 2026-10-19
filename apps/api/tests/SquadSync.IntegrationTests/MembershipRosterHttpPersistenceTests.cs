using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SquadSync.Api.Memberships;
using SquadSync.Api.People;
using SquadSync.Api.Rosters;
using SquadSync.Api.Teams;
using SquadSync.Infrastructure.Persistence;
using Xunit;
using Xunit.Sdk;

namespace SquadSync.IntegrationTests;

[Collection("Transactional HTTP database")]
public sealed class MembershipRosterHttpPersistenceTests
{
    [DatabaseFact]
    [Trait("Category", "Database")]
    public async Task WorkflowAndRejectedRequests_UseRealPostgres_AndPreserveAllOriginalRows()
    {
        var database = new TransactionalHttpDatabase();
        await database.RunAsync(async (client, db) =>
        {
            var workflow = await CreateAndReadAsync(database, client, db);
            var team = workflow.Team.Id;
            var otherTeam = workflow.OtherTeam.Id;
            var player = workflow.Memberships.First(member => member.TeamRole == "Player");
            var coach = workflow.Memberships.Single(member => member.TeamRole == "Coach");
            var entry = workflow.Entries[0];

            // Each check fingerprints every complete row, not just our new records. Real
            // duplicate inserts exercise EF savepoints inside the owned outer transaction.
            await RejectPostAsync($"/api/teams/{team}/memberships",
                new { personId = player.Person.Id, teamRole = "Coach" }, HttpStatusCode.Conflict);
            await RejectPostAsync($"/api/teams/{team}/roster",
                new { teamMembershipId = entry.TeamMembershipId, rosterStatus = "Active" }, HttpStatusCode.Conflict);
            await RejectPostAsync($"/api/teams/{team}/roster",
                new { teamMembershipId = coach.Id, rosterStatus = "Active" }, HttpStatusCode.Conflict);
            await RejectPostAsync($"/api/teams/{otherTeam}/roster",
                new { teamMembershipId = player.Id, rosterStatus = "Active" }, HttpStatusCode.NotFound);
            await RejectGetAsync($"/api/teams/{otherTeam}/memberships/{player.Id}", HttpStatusCode.NotFound);
            await RejectGetAsync($"/api/teams/{otherTeam}/roster/{entry.Id}", HttpStatusCode.NotFound);

            await RejectPostAsync($"/api/teams/{team}/memberships",
                new { personId = "not-a-guid", teamRole = "Player" }, HttpStatusCode.BadRequest);
            await RejectPostAsync($"/api/teams/{team}/roster",
                new { teamMembershipId = "not-a-guid", rosterStatus = "Active" }, HttpStatusCode.BadRequest);
            await RejectPostAsync($"/api/teams/{team}/roster",
                new { teamMembershipId = player.Id, rosterStatus = "active" }, HttpStatusCode.BadRequest);
            await RejectPostAsync($"/api/teams/{team}/roster",
                new { teamMembershipId = player.Id, rosterStatus = 0 }, HttpStatusCode.BadRequest);
            await RejectGetAsync($"/api/teams/not-a-guid/memberships", HttpStatusCode.BadRequest);
            await RejectGetAsync($"/api/teams/{Guid.Empty}/roster", HttpStatusCode.BadRequest);
            await RejectGetAsync($"/api/teams/{team}/roster/not-a-guid", HttpStatusCode.BadRequest);

            // Failed writes must leave the transaction usable for subsequent HTTP reads.
            await AssertCollectionsAsync(database, client, workflow);
            Assert.Equal(database.ContextIds.Count, database.ContextIds.Distinct().Count());

            async Task RejectPostAsync(string route, object payload, HttpStatusCode status)
            {
                var before = await TransactionalHttpDatabase.SnapshotAsync(db);
                using var response = await client.PostAsJsonAsync(route, payload);
                await AssertProblemAsync(response, status);
                Assert.Equal(before, await TransactionalHttpDatabase.SnapshotAsync(db));
            }

            async Task RejectGetAsync(string route, HttpStatusCode status)
            {
                var before = await TransactionalHttpDatabase.SnapshotAsync(db);
                using var response = await client.GetAsync(route);
                await AssertProblemAsync(response, status);
                Assert.Equal(before, await TransactionalHttpDatabase.SnapshotAsync(db));
            }
        });
    }

    [DatabaseFact]
    [Trait("Category", "Database")]
    public async Task AssertionFailureAfterMembershipAndRosterHttpWrites_RollsBackEveryRequest()
    {
        var database = new TransactionalHttpDatabase();
        var failure = await Assert.ThrowsAsync<FailException>(() => database.RunAsync(async (client, db) =>
        {
            await CreateAndReadAsync(database, client, db);
            Assert.Fail("Deliberate failure after membership and roster HTTP writes and reads.");
        }));
        // Cleanup failures must not masquerade as the intended failure.
        Assert.Equal("Deliberate failure after membership and roster HTTP writes and reads.", failure.Message);
    }

    private static async Task<Workflow> CreateAndReadAsync(
        TransactionalHttpDatabase database, HttpClient client, SquadSyncDbContext db)
    {
        var team = await CreateAsync<TeamResponse>(database, client, "/api/teams", new { name = "HTTP Roster FC" });
        var otherTeam = await CreateAsync<TeamResponse>(database, client, "/api/teams", new { name = "Other HTTP FC" });
        var memberships = new List<MembershipResponse>();
        var entries = new List<RosterEntryResponse>();
        // Deliberately create in a different order from the collection contract. Identical
        // names exercise the final ID tie-breaker; all status values are persisted.
        foreach (var (first, last, role, status, jersey) in new (string, string, string, string?, string?)[]
        {
            ("Zoe", "Zulu", "Player", "Active", "007"),
            ("Bea", "Alpha", "Player", "Reserved", null),
            ("Alex", "Alpha", "Player", "Suspended", "12"),
            ("Alex", "Alpha", "Player", "Active", "9"),
            ("Unrostered", "Player", "Player", null, null),
            ("Staff", "Coach", "Coach", null, null)
        })
        {
            var person = await CreateAsync<PersonResponse>(database, client, "/api/people",
                new { firstName = first, lastName = last });
            var persistedPerson = await db.People.AsNoTracking().SingleAsync(candidate => candidate.Id == person.Id);
            Assert.Equal(first, persistedPerson.FirstName);
            Assert.Equal(last, persistedPerson.LastName);

            var member = await CreateAsync<MembershipResponse>(database, client, $"/api/teams/{team.Id}/memberships",
                new { personId = person.Id, teamRole = role });
            Assert.Equal(team.Id, member.TeamId);
            Assert.Equal(new MembershipPersonResponse(person.Id, first, last), member.Person);
            Assert.Equal(role, member.TeamRole);
            memberships.Add(member);
            var persistedMember = await db.TeamMemberships.AsNoTracking().SingleAsync(candidate => candidate.Id == member.Id);
            Assert.Equal(team.Id, persistedMember.TeamId);
            Assert.Equal(person.Id, persistedMember.PersonId);
            Assert.Equal(role, persistedMember.TeamRole.ToString());

            if (status is null) continue;
            var entry = await CreateAsync<RosterEntryResponse>(database, client, $"/api/teams/{team.Id}/roster",
                new { teamMembershipId = member.Id, jerseyNumber = jersey, rosterStatus = status });
            Assert.Equal(member.Id, entry.TeamMembershipId);
            Assert.Equal(new RosterPersonResponse(person.Id, first, last), entry.Person);
            Assert.Equal(jersey, entry.JerseyNumber);
            Assert.Equal(status, entry.RosterStatus);
            entries.Add(entry);
            var persistedEntry = await db.RosterEntries.AsNoTracking().SingleAsync(candidate => candidate.Id == entry.Id);
            Assert.Equal(member.Id, persistedEntry.TeamMembershipId);
            Assert.Equal(jersey, persistedEntry.JerseyNumber);
            Assert.Equal(status, persistedEntry.RosterStatus.ToString());
        }

        // The same Person can be rostered on another Team without leaking into this list.
        var otherMember = await CreateAsync<MembershipResponse>(database, client, $"/api/teams/{otherTeam.Id}/memberships",
            new { personId = memberships[0].Person.Id, teamRole = "Player" });
        var otherEntry = await CreateAsync<RosterEntryResponse>(database, client, $"/api/teams/{otherTeam.Id}/roster",
            new { teamMembershipId = otherMember.Id, jerseyNumber = "99", rosterStatus = "Reserved" });
        Assert.Equal(otherTeam.Name, (await db.Teams.AsNoTracking().SingleAsync(candidate => candidate.Id == otherTeam.Id)).Name);
        Assert.Equal(team.Name, (await db.Teams.AsNoTracking().SingleAsync(candidate => candidate.Id == team.Id)).Name);
        var workflow = new Workflow(team, otherTeam, memberships.ToArray(), entries.ToArray());
        await AssertCollectionsAsync(database, client, workflow);
        Assert.Equal(new[] { otherMember }, await GetAsync<MembershipResponse[]>(database, client, $"/api/teams/{otherTeam.Id}/memberships"));
        Assert.Equal(new[] { otherEntry }, await GetAsync<RosterEntryResponse[]>(database, client, $"/api/teams/{otherTeam.Id}/roster"));
        Assert.Equal(database.ContextIds.Count, database.ContextIds.Distinct().Count());
        return workflow;
    }

    private static async Task AssertCollectionsAsync(TransactionalHttpDatabase database, HttpClient client, Workflow workflow)
    {
        var members = await GetAsync<MembershipResponse[]>(database, client, $"/api/teams/{workflow.Team.Id}/memberships");
        Assert.Equal(workflow.Memberships.OrderBy(member => member.Person.LastName, StringComparer.Ordinal)
            .ThenBy(member => member.Person.FirstName, StringComparer.Ordinal).ThenBy(member => member.Id), members);
        var roster = await GetAsync<RosterEntryResponse[]>(database, client, $"/api/teams/{workflow.Team.Id}/roster");
        Assert.Equal(workflow.Entries.OrderBy(entry => entry.Person.LastName, StringComparer.Ordinal)
            .ThenBy(entry => entry.Person.FirstName, StringComparer.Ordinal).ThenBy(entry => entry.Id), roster);
        // Exact list equality excludes staff, unrostered Players and the other Team's entry.
        Assert.All(roster, entry => Assert.Contains(members, member => member.Id == entry.TeamMembershipId && member.TeamRole == "Player"));
    }

    private static async Task<T> CreateAsync<T>(TransactionalHttpDatabase database, HttpClient client, string route, object payload)
        where T : class
    {
        var contextsBefore = database.ContextIds.Count;
        using var response = await client.PostAsJsonAsync(route, payload);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(contextsBefore + 1, database.ContextIds.Count);
        var resource = (await response.Content.ReadFromJsonAsync<T>())!;
        var id = resource switch
        {
            PersonResponse person => person.Id,
            TeamResponse team => team.Id,
            MembershipResponse member => member.Id,
            RosterEntryResponse entry => entry.Id,
            _ => throw new InvalidOperationException("Unexpected resource type.")
        };
        Assert.NotEqual(Guid.Empty, id);
        Assert.Equal($"{route}/{id}", response.Headers.Location!.AbsolutePath);
        Assert.Equal(resource, await GetAsync<T>(database, client, response.Headers.Location.ToString()));
        return resource;
    }

    private static async Task<T> GetAsync<T>(TransactionalHttpDatabase database, HttpClient client, string route)
    {
        var contextsBefore = database.ContextIds.Count;
        using var response = await client.GetAsync(route);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(contextsBefore + 1, database.ContextIds.Count);
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }

    private static async Task AssertProblemAsync(HttpResponseMessage response, HttpStatusCode status)
    {
        Assert.Equal(status, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var body = await response.Content.ReadAsStringAsync();
        var problem = JsonSerializer.Deserialize<JsonElement>(body);
        Assert.Equal((int)status, problem.GetProperty("status").GetInt32());
        Assert.False(string.IsNullOrWhiteSpace(problem.GetProperty("title").GetString()));
        Assert.False(string.IsNullOrWhiteSpace(problem.GetProperty("type").GetString()));
        if (status == HttpStatusCode.BadRequest)
            Assert.True(problem.GetProperty("errors").EnumerateObject().Any());
        foreach (var forbidden in new[] { "Npgsql", "PostgresException", "DbUpdateException", "Microsoft.EntityFrameworkCore",
                     "SQLSTATE", "23505", "IX_", "FK_", "stackTrace", "password", "Host=", "ConnectionStrings" })
            Assert.DoesNotContain(forbidden, body, StringComparison.OrdinalIgnoreCase);
    }

    private sealed record Workflow(TeamResponse Team, TeamResponse OtherTeam,
        MembershipResponse[] Memberships, RosterEntryResponse[] Entries);
}
