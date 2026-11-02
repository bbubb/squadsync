using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SquadSync.Api.Teams;
using SquadSync.Application.Teams;
using SquadSync.Domain;
using Xunit;

namespace SquadSync.IntegrationTests;

public class TeamsEndpointTests
{
    [Fact]
    public async Task GetAll_EmptyStorage_ReturnsOkEmptyArray()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/api/teams");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal("[]", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task GetAll_ReturnsOrderedMinimalTeamResponses()
    {
        var persistence = new TestTeamPersistence();
        var teams = new[]
        {
            new Team(Guid.Parse("a0000000-0000-0000-0000-000000000002"), "Alpha FC"),
            new Team(Guid.NewGuid(), "Zulu FC"),
            new Team(Guid.Parse("a0000000-0000-0000-0000-000000000001"), "Alpha FC")
        };
        foreach (var team in teams) await persistence.AddAsync(team);
        await using var factory = CreateFactory(persistence: persistence);
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/api/teams");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(new[] { teams[2].Id, teams[0].Id, teams[1].Id },
            json.EnumerateArray().Select(team => team.GetProperty("id").GetGuid()));
        Assert.Equal(new[] { "Alpha FC", "Alpha FC", "Zulu FC" },
            json.EnumerateArray().Select(team => team.GetProperty("name").GetString()));
        foreach (var team in json.EnumerateArray())
            Assert.Equal(new[] { "id", "name" }, team.EnumerateObject().Select(property => property.Name).Order());
    }

    [Fact]
    public async Task CreateAndGet_UseRealApplicationAndReturnMinimalNormalizedResource()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var created = await client.PostAsJsonAsync("/api/teams", new { name = "  SquadSync FC\t" });

        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var team = (await created.Content.ReadFromJsonAsync<TeamResponse>())!;
        Assert.NotEqual(Guid.Empty, team.Id);
        Assert.Equal("SquadSync FC", team.Name);
        Assert.Equal($"/api/teams/{team.Id}", created.Headers.Location!.AbsolutePath);

        var retrieved = await client.GetAsync(created.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, retrieved.StatusCode);
        var body = await retrieved.Content.ReadAsStringAsync();
        Assert.Equal(team, JsonSerializer.Deserialize<TeamResponse>(body, JsonSerializerOptions.Web));
        var json = JsonSerializer.Deserialize<JsonElement>(body);
        Assert.Equal(new[] { "id", "name" }, json.EnumerateObject().Select(p => p.Name).Order().ToArray());
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("null")]
    [InlineData("")]
    [InlineData("{\"name\":")]
    [InlineData("{\"name\":42}")]
    [InlineData("{\"name\":null}")]
    [InlineData("{\"name\":\"\"}")]
    [InlineData("{\"name\":\" \\t\"}")]
    public async Task InvalidCreate_ReturnsValidationProblemWithoutSaving(string payload)
    {
        var persistence = new TestTeamPersistence();
        await using var factory = CreateFactory(persistence: persistence);
        using var client = factory.CreateClient();

        var response = await client.PostAsync("/api/teams", new StringContent(payload, Encoding.UTF8, "application/json"));

        var problem = await AssertProblem(response, HttpStatusCode.BadRequest);
        Assert.True(problem.GetProperty("errors").EnumerateObject().Any());
        Assert.Equal(0, persistence.Count);
    }

    [Fact]
    public async Task MissingTeam_ReturnsNotFoundProblem()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();
        await AssertProblem(await client.GetAsync($"/api/teams/{Guid.NewGuid()}"), HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task MalformedId_ReturnsValidationProblem()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();
        var problem = await AssertProblem(await client.GetAsync("/api/teams/not-a-guid"), HttpStatusCode.BadRequest);
        Assert.True(problem.GetProperty("errors").EnumerateObject().Any());
    }

    [Theory]
    [InlineData("Staging")]
    [InlineData("Production")]
    public async Task TeamRoutes_AreUnavailableOutsideDevelopmentAndHealthRemainsAvailable(string environment)
    {
        var persistence = new TestTeamPersistence();
        await using var factory = CreateFactory(environment, persistence);
        using var client = factory.CreateClient();

        Assert.Equal(HttpStatusCode.NotFound,
            (await client.PostAsJsonAsync("/api/teams", new { name = "SquadSync FC" })).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/teams/{Guid.NewGuid()}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/api/teams")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/swagger/v1/swagger.json")).StatusCode);
        Assert.Equal(0, persistence.Count);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/health")).StatusCode);
        Assert.Equal(HttpStatusCode.ServiceUnavailable, (await client.GetAsync("/health/ready")).StatusCode);
    }

    private static Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program> CreateFactory(
        string environment = "Development", TestTeamPersistence? persistence = null)
        => new ApiTestFactory(environment).WithWebHostBuilder(builder => builder.ConfigureServices(services =>
        {
            services.RemoveAll<ITeamPersistence>();
            services.AddSingleton<ITeamPersistence>(persistence ?? new TestTeamPersistence());
            services.RemoveAll<ITeamQueries>();
            services.AddSingleton<ITeamQueries>(provider => (TestTeamPersistence)provider.GetRequiredService<ITeamPersistence>());
        }));

    private static async Task<JsonElement> AssertProblem(HttpResponseMessage response, HttpStatusCode status)
    {
        Assert.Equal(status, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var problem = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal((int)status, problem.GetProperty("status").GetInt32());
        Assert.False(string.IsNullOrWhiteSpace(problem.GetProperty("type").GetString()));
        Assert.False(string.IsNullOrWhiteSpace(problem.GetProperty("title").GetString()));
        return problem;
    }

    private sealed class TestTeamPersistence : ITeamPersistence, ITeamQueries
    {
        private readonly ConcurrentDictionary<Guid, Team> teams = new();
        public int Count => teams.Count;

        public Task<IReadOnlyList<TeamDetails>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult<IReadOnlyList<TeamDetails>>(teams.Values
                .OrderBy(team => team.Name, StringComparer.Ordinal).ThenBy(team => team.Id)
                .Select(team => new TeamDetails(team.Id, team.Name)).ToArray());
        }

        public Task AddAsync(Team team, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Assert.True(teams.TryAdd(team.Id, team));
            return Task.CompletedTask;
        }

        public Task<Team?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            teams.TryGetValue(id, out var team);
            return Task.FromResult(team);
        }
    }
}
