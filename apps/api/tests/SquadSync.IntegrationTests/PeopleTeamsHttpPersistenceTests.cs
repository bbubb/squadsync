using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SquadSync.Api.People;
using SquadSync.Api.Teams;
using SquadSync.Infrastructure.Persistence;
using Xunit;
using Xunit.Sdk;

namespace SquadSync.IntegrationTests;

[Collection("Transactional HTTP database")]
public sealed class PeopleTeamsHttpPersistenceTests
{
    [DatabaseFact]
    [Trait("Category", "Database")]
    public async Task CreateAndReadAcrossRequests_UseRealPostgres_AndLeaveAllOriginalRowsIntact()
    {
        var database = new TransactionalHttpDatabase();
        await database.RunAsync(async (client, db) =>
        {
            Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/health")).StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/health/ready")).StatusCode);
            await CreateAndReadAsync(database, client, db);
            var beforeInvalid = await TransactionalHttpDatabase.SnapshotAsync(db);

            // Exhaustive payload cases stay in the ordinary contract tests. These representatives
            // add the database-specific guarantee that invalid HTTP requests change no rows.
            foreach (var (route, payload) in new[]
            {
                ("/api/people", "{\"firstName\":"),
                ("/api/people", "{\"firstName\":\" \\t\",\"lastName\":\"Player\"}"),
                ("/api/teams", "{\"name\":"),
                ("/api/teams", "{\"name\":\" \\t\"}")
            })
            {
                using var response = await client.PostAsync(route,
                    new StringContent(payload, Encoding.UTF8, "application/json"));
                var problem = await AssertProblemAsync(response, HttpStatusCode.BadRequest);
                Assert.True(problem.GetProperty("errors").EnumerateObject().Any());
                Assert.Equal(beforeInvalid, await TransactionalHttpDatabase.SnapshotAsync(db));
            }

            var absentId = Guid.NewGuid();
            Assert.False(await db.People.AnyAsync(person => person.Id == absentId));
            Assert.False(await db.Teams.AnyAsync(team => team.Id == absentId));
            foreach (var route in new[] { "/api/people", "/api/teams" })
            {
                using var response = await client.GetAsync($"{route}/{absentId}");
                await AssertProblemAsync(response, HttpStatusCode.NotFound);
            }
            Assert.Equal(beforeInvalid, await TransactionalHttpDatabase.SnapshotAsync(db));
        });
    }

    [DatabaseFact]
    [Trait("Category", "Database")]
    public async Task AssertionFailureAfterSuccessfulHttpWrites_RollsBackEveryRequest()
    {
        var database = new TransactionalHttpDatabase();
        var failure = await Assert.ThrowsAsync<FailException>(() => database.RunAsync(async (client, db) =>
        {
            await CreateAndReadAsync(database, client, db);
            Assert.Fail("Deliberate failure after both HTTP writes and reads.");
        }));
        // A cleanup assertion cannot silently replace the injected failure and make this pass.
        Assert.Equal("Deliberate failure after both HTTP writes and reads.", failure.Message);
    }

    private static async Task CreateAndReadAsync(
        TransactionalHttpDatabase database, HttpClient client, SquadSyncDbContext db)
    {
        var peopleBefore = await db.People.CountAsync();
        var teamsBefore = await db.Teams.CountAsync();
        var contextCount = database.ContextIds.Count;
        using var personCreated = await client.PostAsJsonAsync("/api/people",
            new { firstName = "  HTTP ", lastName = " Person\t" });
        Assert.Equal(HttpStatusCode.Created, personCreated.StatusCode);
        var person = (await personCreated.Content.ReadFromJsonAsync<PersonResponse>())!;
        AssertGeneratedId(person.Id);
        Assert.Equal("HTTP", person.FirstName);
        Assert.Equal("Person", person.LastName);
        Assert.Equal($"/api/people/{person.Id}", personCreated.Headers.Location!.AbsolutePath);
        Assert.Equal(contextCount + 1, database.ContextIds.Count);
        using var personRead = await client.GetAsync(personCreated.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, personRead.StatusCode);
        Assert.Equal(person, await personRead.Content.ReadFromJsonAsync<PersonResponse>());
        Assert.Equal(contextCount + 2, database.ContextIds.Count);
        var persistedPerson = await db.People.AsNoTracking().SingleAsync(candidate => candidate.Id == person.Id);
        Assert.Equal(person.FirstName, persistedPerson.FirstName);
        Assert.Equal(person.LastName, persistedPerson.LastName);
        Assert.Equal(peopleBefore + 1, await db.People.CountAsync());

        using var teamCreated = await client.PostAsJsonAsync("/api/teams", new { name = "  HTTP Validation FC\t" });
        Assert.Equal(HttpStatusCode.Created, teamCreated.StatusCode);
        var team = (await teamCreated.Content.ReadFromJsonAsync<TeamResponse>())!;
        AssertGeneratedId(team.Id);
        Assert.NotEqual(person.Id, team.Id);
        Assert.Equal("HTTP Validation FC", team.Name);
        Assert.Equal($"/api/teams/{team.Id}", teamCreated.Headers.Location!.AbsolutePath);
        Assert.Equal(contextCount + 3, database.ContextIds.Count);
        using var teamRead = await client.GetAsync(teamCreated.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, teamRead.StatusCode);
        Assert.Equal(team, await teamRead.Content.ReadFromJsonAsync<TeamResponse>());
        Assert.Equal(contextCount + 4, database.ContextIds.Count);
        Assert.Equal(team.Name, (await db.Teams.AsNoTracking().SingleAsync(candidate => candidate.Id == team.Id)).Name);
        Assert.Equal(teamsBefore + 1, await db.Teams.CountAsync());
        // Includes all four request scopes and the host registration probe; none reuse a context.
        Assert.Equal(database.ContextIds.Count, database.ContextIds.Distinct().Count());
    }

    private static void AssertGeneratedId(Guid id)
    {
        Assert.NotEqual(Guid.Empty, id);
        Assert.False(id.ToString().StartsWith("11100000-0000-0000-0000-", StringComparison.Ordinal));
    }

    private static async Task<JsonElement> AssertProblemAsync(HttpResponseMessage response, HttpStatusCode status)
    {
        Assert.Equal(status, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var problem = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal((int)status, problem.GetProperty("status").GetInt32());
        Assert.False(string.IsNullOrWhiteSpace(problem.GetProperty("type").GetString()));
        Assert.False(string.IsNullOrWhiteSpace(problem.GetProperty("title").GetString()));
        return problem;
    }
}
