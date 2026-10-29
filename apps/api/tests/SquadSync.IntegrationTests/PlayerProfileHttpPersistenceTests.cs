using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SquadSync.Api.Memberships;
using SquadSync.Api.People;
using SquadSync.Api.PlayerProfiles;
using SquadSync.Api.Rosters;
using SquadSync.Api.Teams;
using SquadSync.Infrastructure.Persistence;
using Xunit;
using Xunit.Sdk;

namespace SquadSync.IntegrationTests;

[Collection("Transactional HTTP database")]
public sealed class PlayerProfileHttpPersistenceTests
{
    [DatabaseFact]
    [Trait("Category", "Database")]
    public async Task Workflow_UsesSeparateRequestContexts_AndPreservesOriginalRows()
    {
        var database = new TransactionalHttpDatabase();
        await database.RunAsync((client, db) => CreateAndUpdateAsync(database, client, db));
    }

    [DatabaseFact]
    [Trait("Category", "Database")]
    public async Task RejectedRequests_ReturnSafeProblems_AndPreserveEveryRow()
    {
        var database = new TransactionalHttpDatabase();
        await database.RunAsync(async (client, db) =>
        {
            var person = await CreateAsync<PersonResponse>(database, client, "/api/people",
                new { firstName = "Rejected", lastName = "Profile" });
            var route = Route(person.Id);
            var missing = Route(Guid.NewGuid());
            const string valid = """{"dominantFoot":"Right","heightInches":70,"weightPounds":152.5}""";

            await RejectAsync(HttpMethod.Get, route, null, HttpStatusCode.NotFound);
            await RejectAsync(HttpMethod.Put, route, valid, HttpStatusCode.NotFound);
            Assert.False(await db.PlayerProfiles.AnyAsync(profile => profile.PersonId == person.Id));
            foreach (var method in new[] { HttpMethod.Get, HttpMethod.Post, HttpMethod.Put })
            {
                await RejectAsync(method, missing, method == HttpMethod.Get ? null : valid, HttpStatusCode.NotFound);
                foreach (var id in new[] { "not-a-guid", Guid.Empty.ToString() })
                    await RejectAsync(method, $"/api/people/{id}/player-profile",
                        method == HttpMethod.Get ? null : valid, HttpStatusCode.BadRequest);
            }

            var original = await CreateAsync<PlayerProfileResponse>(database, client, route,
                new { dominantFoot = "Right", heightInches = 70, weightPounds = 152.5m });
            await RejectAsync(HttpMethod.Post, route, valid, HttpStatusCode.Conflict);

            foreach (var method in new[] { HttpMethod.Post, HttpMethod.Put })
            {
                foreach (var payload in new[]
                {
                    "{", "null", "[]",
                    """{"dominantFoot":"left","heightInches":70,"weightPounds":152.5}""",
                    """{"dominantFoot":"Unknown","heightInches":70,"weightPounds":152.5}""",
                    """{"dominantFoot":1,"heightInches":70,"weightPounds":152.5}""",
                    """{"dominantFoot":"Left","heightInches":0,"weightPounds":152.5}""",
                    """{"dominantFoot":"Left","heightInches":-1,"weightPounds":152.5}""",
                    """{"dominantFoot":"Left","heightInches":70,"weightPounds":0}""",
                    """{"dominantFoot":"Left","heightInches":70,"weightPounds":-1}""",
                    """{"dominantFoot":"Left","heightInches":"70","weightPounds":152.5}""",
                    """{"dominantFoot":"Left","heightInches":70,"weightPounds":"152.5"}""",
                    """{"id":"00000000-0000-0000-0000-000000000001","dominantFoot":null,"heightInches":null,"weightPounds":null}"""
                })
                    await RejectAsync(method, route, payload, HttpStatusCode.BadRequest);
            }
            foreach (var payload in new[]
            {
                "{}", """{"heightInches":70,"weightPounds":152.5}""",
                """{"dominantFoot":"Left","weightPounds":152.5}""",
                """{"dominantFoot":"Left","heightInches":70}"""
            })
                await RejectAsync(HttpMethod.Put, route, payload, HttpStatusCode.BadRequest);

            Assert.Equal(original, await GetAsync<PlayerProfileResponse>(database, client, route));
            Assert.Equal(1, await db.PlayerProfiles.CountAsync(profile => profile.PersonId == person.Id));
            Assert.Equal(database.ContextIds.Count, database.ContextIds.Distinct().Count());

            async Task RejectAsync(HttpMethod method, string path, string? payload, HttpStatusCode status)
            {
                var before = await TransactionalHttpDatabase.SnapshotAsync(db);
                using var request = new HttpRequestMessage(method, path);
                if (payload is not null)
                    request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                using var response = await client.SendAsync(request);
                await AssertProblemAsync(response, status);
                Assert.Equal(before, await TransactionalHttpDatabase.SnapshotAsync(db));
            }
        });
    }

    [DatabaseFact]
    [Trait("Category", "Database")]
    public async Task AssertionFailureAfterProfileHttpWrites_RollsBackEveryRequest()
    {
        var database = new TransactionalHttpDatabase();
        var failure = await Assert.ThrowsAsync<FailException>(() => database.RunAsync(async (client, db) =>
        {
            await CreateAndUpdateAsync(database, client, db);
            Assert.Fail("Deliberate failure after profile HTTP creation, updates and reads.");
        }));
        // A rollback/fingerprint failure must not be mistaken for the injected assertion.
        Assert.Equal("Deliberate failure after profile HTTP creation, updates and reads.", failure.Message);
    }

    private static async Task CreateAndUpdateAsync(
        TransactionalHttpDatabase database, HttpClient client, SquadSyncDbContext db)
    {
        var person = await CreateAsync<PersonResponse>(database, client, "/api/people",
            new { firstName = "Shared", lastName = "Player" });
        Assert.False(await db.PlayerProfiles.AnyAsync(profile => profile.PersonId == person.Id));

        // Explicit blank POST needs no membership. Omitted optional values persist as null.
        var blank = await CreateAsync<PlayerProfileResponse>(database, client, Route(person.Id), new { });
        Assert.Equal(new PlayerProfileResponse(blank.Id, person.Id, null, null, null), blank);
        await AssertPersistedAsync(db, blank);

        var unprofiled = await CreateAsync<PersonResponse>(database, client, "/api/people",
            new { firstName = "Unprofiled", lastName = "Player" });
        var teams = new List<(TeamResponse Team, MembershipResponse[] Members, RosterEntryResponse[] Entries)>();
        foreach (var name in new[] { "Profile Home FC", "Profile Away FC" })
        {
            var team = await CreateAsync<TeamResponse>(database, client, "/api/teams", new { name });
            var members = new List<MembershipResponse>();
            var entries = new List<RosterEntryResponse>();
            foreach (var player in new[] { person, unprofiled })
            {
                var member = await CreateAsync<MembershipResponse>(database, client, $"/api/teams/{team.Id}/memberships",
                    new { personId = player.Id, teamRole = "Player" });
                Assert.Equal(new MembershipPersonResponse(player.Id, player.FirstName, player.LastName), member.Person);
                Assert.Equal(team.Id, member.TeamId);
                Assert.Equal("Player", member.TeamRole);
                members.Add(member);
                var entry = await CreateAsync<RosterEntryResponse>(database, client, $"/api/teams/{team.Id}/roster",
                    new { teamMembershipId = member.Id, jerseyNumber = "007", rosterStatus = "Active" });
                Assert.Equal(member.Id, entry.TeamMembershipId);
                Assert.Equal(new RosterPersonResponse(player.Id, player.FirstName, player.LastName), entry.Person);
                Assert.Equal("007", entry.JerseyNumber);
                Assert.Equal("Active", entry.RosterStatus);
                entries.Add(entry);
                Assert.False(await db.PlayerProfiles.AnyAsync(profile => profile.PersonId == unprofiled.Id));
            }
            teams.Add((team, members.ToArray(), entries.ToArray()));
        }

        // Full replacement, selective clearing and complete clearing all retain both IDs.
        foreach (var expected in new[]
        {
            blank with { DominantFoot = "Left", HeightInches = 70, WeightPounds = 152.123456789m },
            blank with { DominantFoot = "Both", HeightInches = 72, WeightPounds = 175.25m },
            blank with { HeightInches = 71 },
            blank
        })
        {
            var before = database.ContextIds.Count;
            using var response = await client.PutAsJsonAsync(Route(person.Id), new
            {
                expected.DominantFoot, expected.HeightInches, expected.WeightPounds
            });
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(before + 1, database.ContextIds.Count);
            Assert.Equal(expected, await response.Content.ReadFromJsonAsync<PlayerProfileResponse>());
            Assert.Equal(expected, await GetAsync<PlayerProfileResponse>(database, client, Route(person.Id)));
            await AssertPersistedAsync(db, expected);
        }

        Assert.Equal(2, await db.TeamMemberships.CountAsync(member => member.PersonId == person.Id));
        Assert.Equal(1, await db.PlayerProfiles.CountAsync(profile => profile.PersonId == person.Id));
        Assert.False(await db.PlayerProfiles.AnyAsync(profile => profile.PersonId == unprofiled.Id));
        foreach (var (team, members, entries) in teams)
        {
            Assert.Equal(members.OrderBy(member => member.Person.LastName, StringComparer.Ordinal)
                .ThenBy(member => member.Person.FirstName, StringComparer.Ordinal).ThenBy(member => member.Id),
                await GetAsync<MembershipResponse[]>(database, client, $"/api/teams/{team.Id}/memberships"));
            Assert.Equal(entries.OrderBy(entry => entry.Person.LastName, StringComparer.Ordinal)
                .ThenBy(entry => entry.Person.FirstName, StringComparer.Ordinal).ThenBy(entry => entry.Id),
                await GetAsync<RosterEntryResponse[]>(database, client, $"/api/teams/{team.Id}/roster"));
        }
        Assert.Equal(database.ContextIds.Count, database.ContextIds.Distinct().Count());
    }

    private static async Task AssertPersistedAsync(SquadSyncDbContext db, PlayerProfileResponse expected)
    {
        var persisted = await db.PlayerProfiles.AsNoTracking().SingleAsync(profile => profile.PersonId == expected.PersonId);
        Assert.Equal(expected, new PlayerProfileResponse(persisted.Id, persisted.PersonId,
            persisted.DominantFoot?.ToString(), persisted.HeightInches, persisted.WeightPounds));
    }

    private static string Route(Guid personId) => $"/api/people/{personId}/player-profile";

    private static async Task<T> CreateAsync<T>(
        TransactionalHttpDatabase database, HttpClient client, string route, object payload) where T : class
    {
        var before = database.ContextIds.Count;
        using var response = await client.PostAsJsonAsync(route, payload);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(before + 1, database.ContextIds.Count);
        var resource = (await response.Content.ReadFromJsonAsync<T>())!;
        var location = response.Headers.Location!;
        var expectedPath = resource switch
        {
            PlayerProfileResponse profile => Route(profile.PersonId),
            PersonResponse person => $"{route}/{person.Id}",
            TeamResponse team => $"{route}/{team.Id}",
            MembershipResponse member => $"{route}/{member.Id}",
            RosterEntryResponse entry => $"{route}/{entry.Id}",
            _ => throw new InvalidOperationException("Unexpected resource type.")
        };
        Assert.Equal(expectedPath, location.AbsolutePath);
        if (resource is PlayerProfileResponse created) Assert.NotEqual(Guid.Empty, created.Id);
        Assert.Equal(resource, await GetAsync<T>(database, client, location.ToString()));
        return resource;
    }

    private static async Task<T> GetAsync<T>(TransactionalHttpDatabase database, HttpClient client, string route)
    {
        var before = database.ContextIds.Count;
        using var response = await client.GetAsync(route);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(before + 1, database.ContextIds.Count);
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
}
