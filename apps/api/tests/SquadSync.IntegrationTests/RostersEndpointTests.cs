using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SquadSync.Api.Rosters;
using SquadSync.Application.Memberships;
using SquadSync.Application.Rosters;
using SquadSync.Application.Teams;
using SquadSync.Domain;
using Xunit;

namespace SquadSync.IntegrationTests;

public class RostersEndpointTests
{
    [Theory]
    [InlineData("Active")]
    [InlineData("Reserved")]
    [InlineData("Suspended")]
    public async Task Create_LocationReturnsSameRosterAndPersonSummary(string status)
    {
        var fake = new Persistence();
        await using var factory = CreateFactory(fake);
        using var client = factory.CreateClient();
        var created = await client.PostAsJsonAsync(Route(fake.Team.Id), new { teamMembershipId = fake.Membership.Id, jerseyNumber = "007", rosterStatus = status });
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var entry = (await created.Content.ReadFromJsonAsync<RosterEntryResponse>())!;
        Assert.NotEqual(Guid.Empty, entry.Id);
        Assert.Equal(fake.Membership.Id, entry.TeamMembershipId);
        Assert.Equal("007", entry.JerseyNumber);
        Assert.Equal(new RosterPersonResponse(fake.Person.Id, "Alex", "Player"), entry.Person);
        Assert.Equal(status, entry.RosterStatus);
        Assert.Equal($"{Route(fake.Team.Id)}/{entry.Id}", created.Headers.Location!.AbsolutePath);
        var retrieved = await client.GetAsync(created.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, retrieved.StatusCode);
        var body = await retrieved.Content.ReadAsStringAsync();
        Assert.Equal(entry, JsonSerializer.Deserialize<RosterEntryResponse>(body, JsonSerializerOptions.Web));
        var json = JsonSerializer.Deserialize<JsonElement>(body);
        Assert.Equal(new[] { "id", "jerseyNumber", "person", "rosterStatus", "teamMembershipId" }, json.EnumerateObject().Select(p => p.Name).Order());
        Assert.Equal(new[] { "firstName", "id", "lastName" }, json.GetProperty("person").EnumerateObject().Select(p => p.Name).Order());
        Assert.Single(fake.Entries);

        await AssertProblem(await client.GetAsync($"{Route(Guid.NewGuid())}/{entry.Id}"), HttpStatusCode.NotFound);
        await AssertProblem(await client.PostAsJsonAsync(Route(fake.Team.Id), new { teamMembershipId = fake.Membership.Id, rosterStatus = "Active" }), HttpStatusCode.Conflict);
        Assert.Single(fake.Entries);
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("null")]
    [InlineData("")]
    [InlineData("{\"rosterStatus\":")]
    [InlineData("{\"teamMembershipId\":\"MEMBERSHIP\"}")]
    [InlineData("{\"teamMembershipId\":\"MEMBERSHIP\",\"rosterStatus\":null}")]
    [InlineData("{\"teamMembershipId\":\"MEMBERSHIP\",\"rosterStatus\":0}")]
    [InlineData("{\"teamMembershipId\":\"MEMBERSHIP\",\"rosterStatus\":\"Active\",\"jerseyNumber\":7}")]
    [InlineData("{\"teamMembershipId\":\"MEMBERSHIP\",\"rosterStatus\":\"0\"}")]
    [InlineData("{\"teamMembershipId\":\"MEMBERSHIP\",\"rosterStatus\":\"active\"}")]
    [InlineData("{\"teamMembershipId\":\"MEMBERSHIP\",\"rosterStatus\":\"Unknown\"}")]
    [InlineData("{\"teamMembershipId\":\"MEMBERSHIP\",\"rosterStatus\":\" Active\"}")]
    [InlineData("{\"teamMembershipId\":\"MEMBERSHIP\",\"rosterStatus\":\"Active\\n\"}")]
    [InlineData("{\"teamMembershipId\":\"MEMBERSHIP\",\"rosterStatus\":\"Owner, Active\"}")]
    [InlineData("{\"teamMembershipId\":\"MEMBERSHIP\",\"rosterStatus\":\"\"}")]
    [InlineData("{\"teamMembershipId\":null,\"rosterStatus\":\"Active\"}")]
    [InlineData("{\"rosterStatus\":\"Active\"}")]
    [InlineData("{\"teamMembershipId\":\"not-a-guid\",\"rosterStatus\":\"Active\"}")]
    [InlineData("{\"teamMembershipId\":\"00000000-0000-0000-0000-000000000000\",\"rosterStatus\":\"Active\"}")]
    public async Task InvalidPayload_ReturnsValidationProblemBeforePersistence(string payload)
    {
        var fake = new Persistence();
        await using var factory = CreateFactory(fake);
        using var client = factory.CreateClient();
        var response = await client.PostAsync(Route(fake.Team.Id),
            new StringContent(payload.Replace("MEMBERSHIP", fake.Membership.Id.ToString()), Encoding.UTF8, "application/json"));
        await AssertProblem(response, HttpStatusCode.BadRequest);
        Assert.Empty(fake.Entries);
        Assert.Equal(0, fake.Calls);
    }

    [Theory]
    [InlineData("not-a-guid")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public async Task InvalidRouteIds_Return400BeforePersistence(string id)
    {
        var fake = new Persistence();
        await using var factory = CreateFactory(fake);
        using var client = factory.CreateClient();
        await AssertProblem(await client.PostAsJsonAsync($"/api/teams/{id}/roster",
            new { teamMembershipId = fake.Membership.Id, rosterStatus = "Active" }), HttpStatusCode.BadRequest);
        await AssertProblem(await client.GetAsync($"/api/teams/{id}/roster/{Guid.NewGuid()}"), HttpStatusCode.BadRequest);
        await AssertProblem(await client.GetAsync($"{Route(fake.Team.Id)}/{id}"), HttpStatusCode.BadRequest);
        await AssertProblem(await client.GetAsync($"/api/teams/{id}/roster"), HttpStatusCode.BadRequest);
        Assert.Equal(0, fake.Calls);
    }

    [Fact]
    public async Task MissingReferencesAndMembership_Return404WithoutSaving()
    {
        var fake = new Persistence();
        await using var factory = CreateFactory(fake);
        using var client = factory.CreateClient();
        await AssertProblem(await client.PostAsJsonAsync(Route(Guid.NewGuid()),
            new { teamMembershipId = fake.Membership.Id, rosterStatus = "Active" }), HttpStatusCode.NotFound);
        await AssertProblem(await client.PostAsJsonAsync(Route(fake.Team.Id),
            new { teamMembershipId = Guid.NewGuid(), rosterStatus = "Active" }), HttpStatusCode.NotFound);
        await AssertProblem(await client.GetAsync($"{Route(fake.Team.Id)}/{Guid.NewGuid()}"), HttpStatusCode.NotFound);
        Assert.Empty(fake.Entries);
    }

    [Theory]
    [InlineData("conflict", 409)]
    [InlineData("reference", 404)]
    [InlineData("failure", 500)]
    public async Task WriteRaceAndUnexpectedFailure_AreSanitized(string failure, int status)
    {
        var fake = new Persistence { Failure = failure };
        await using var factory = CreateFactory(fake);
        using var client = factory.CreateClient();
        await AssertProblem(await client.PostAsJsonAsync(Route(fake.Team.Id),
            new { teamMembershipId = fake.Membership.Id, rosterStatus = "Active" }), (HttpStatusCode)status);
        Assert.Empty(fake.Entries);
    }

    [Fact]
    public async Task ReadFailure_IsSanitized()
    {
        var fake = new Persistence { Failure = "failure" };
        await using var factory = CreateFactory(fake);
        using var client = factory.CreateClient();
        await AssertProblem(await client.GetAsync($"{Route(fake.Team.Id)}/{Guid.NewGuid()}"), HttpStatusCode.InternalServerError);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task AbortedRosterRequest_RemainsCancellation(bool create)
    {
        var fake = new Persistence { Failure = "cancel" };
        await using var factory = CreateFactory(fake);
        using var client = factory.CreateClient();
        var response = create
            ? await client.PostAsJsonAsync(Route(fake.Team.Id), new { teamMembershipId = fake.Membership.Id, rosterStatus = "Active" })
            : await client.GetAsync($"{Route(fake.Team.Id)}/{Guid.NewGuid()}");
        Assert.Equal(499, (int)response.StatusCode);
        Assert.Equal(string.Empty, await response.Content.ReadAsStringAsync());
        Assert.Empty(fake.Entries);
    }

    [Theory]
    [InlineData("Staging")]
    [InlineData("Production")]
    public async Task Routes_AreUnmappedOutsideDevelopment(string environment)
    {
        var fake = new Persistence();
        await using var factory = CreateFactory(fake, environment);
        using var client = factory.CreateClient();
        Assert.Equal(HttpStatusCode.NotFound, (await client.PostAsJsonAsync(Route(fake.Team.Id),
            new { teamMembershipId = fake.Membership.Id, rosterStatus = "Active" })).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"{Route(fake.Team.Id)}/{Guid.NewGuid()}")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/health")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync(Route(fake.Team.Id))).StatusCode);
        Assert.Equal(0, fake.Calls);
    }

    [Theory]
    [InlineData(TeamRole.Owner)]
    [InlineData(TeamRole.Coach)]
    [InlineData(TeamRole.AssistantCoach)]
    [InlineData(TeamRole.Manager)]
    [InlineData(TeamRole.Viewer)]
    public async Task NonPlayer_ReturnsConflictButWrongTeamReturnsNotFound(TeamRole role)
    {
        var fake = new Persistence(role);
        await using var factory = CreateFactory(fake);
        using var client = factory.CreateClient();
        var body = new { teamMembershipId = fake.Membership.Id, rosterStatus = "Active" };
        await AssertProblem(await client.PostAsJsonAsync(Route(fake.Team.Id), body), HttpStatusCode.Conflict);
        await AssertProblem(await client.PostAsJsonAsync(Route(Guid.NewGuid()), body), HttpStatusCode.NotFound);
        Assert.Empty(fake.Entries);
    }

    [Theory]
    [InlineData("")]
    [InlineData("1234")]
    [InlineData(" 7")]
    [InlineData("7 ")]
    [InlineData("7\n")]
    [InlineData("A")]
    [InlineData("\uFF11\uFF12")]
    public async Task InvalidJersey_Returns400WithoutPersistence(string jersey)
    {
        var fake = new Persistence();
        await using var factory = CreateFactory(fake);
        using var client = factory.CreateClient();
        await AssertProblem(await client.PostAsJsonAsync(Route(fake.Team.Id),
            new { teamMembershipId = fake.Membership.Id, jerseyNumber = jersey, rosterStatus = "Active" }), HttpStatusCode.BadRequest);
        Assert.Equal(0, fake.Calls);
    }

    [Fact]
    public async Task NullableJersey_IsAcceptedAndListed()
    {
        var fake = new Persistence();
        await using var factory = CreateFactory(fake);
        using var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync(Route(fake.Team.Id),
            new { teamMembershipId = fake.Membership.Id, jerseyNumber = (string?)null, rosterStatus = "Active" });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Null((await response.Content.ReadFromJsonAsync<RosterEntryResponse>())!.JerseyNumber);
        var entries = (await client.GetFromJsonAsync<RosterEntryResponse[]>(Route(fake.Team.Id)))!;
        Assert.Null(Assert.Single(entries).JerseyNumber);
    }

    [Fact]
    public async Task Collection_EmptyAndMissingTeamsAreDistinct()
    {
        var fake = new Persistence();
        await using var factory = CreateFactory(fake);
        using var client = factory.CreateClient();
        var empty = await client.GetAsync(Route(fake.Team.Id));
        Assert.Equal(HttpStatusCode.OK, empty.StatusCode);
        Assert.Equal("[]", await empty.Content.ReadAsStringAsync());
        await AssertProblem(await client.GetAsync(Route(Guid.NewGuid())), HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("Active")]
    [InlineData("Reserved")]
    [InlineData("Suspended")]
    public async Task Collection_ReturnsExistingTypedRepresentation(string status)
    {
        var fake = new Persistence();
        await using var factory = CreateFactory(fake);
        using var client = factory.CreateClient();
        var created = await client.PostAsJsonAsync(Route(fake.Team.Id),
            new { teamMembershipId = fake.Membership.Id, jerseyNumber = "007", rosterStatus = status });
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var entry = (await created.Content.ReadFromJsonAsync<RosterEntryResponse>())!;
        var response = await client.GetAsync(Route(fake.Team.Id));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(entry, Assert.Single((await response.Content.ReadFromJsonAsync<RosterEntryResponse[]>())!));
        await AssertProblem(await client.GetAsync(Route(Guid.NewGuid())), HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Collection_PreservesQueryOrderAndMapsEveryProjection()
    {
        var fake = new Persistence
        {
            CollectionResults = [
                new(Guid.Parse("b0000000-0000-0000-0000-000000000001"), Guid.NewGuid(), new(Guid.NewGuid(), "Chris", "Delta"), null, RosterStatus.Reserved),
                new(Guid.Parse("b0000000-0000-0000-0000-000000000002"), Guid.NewGuid(), new(Guid.NewGuid(), "Chris", "Delta"), "007", RosterStatus.Active),
                new(Guid.NewGuid(), Guid.NewGuid(), new(Guid.NewGuid(), "Aaron", "Echo"), "12", RosterStatus.Suspended)
            ]
        };
        await using var factory = CreateFactory(fake);
        using var client = factory.CreateClient();
        var response = await client.GetAsync(Route(fake.Team.Id));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var entries = (await response.Content.ReadFromJsonAsync<RosterEntryResponse[]>())!;
        Assert.Equal(fake.CollectionResults.Select(RosterEntryResponse.From), entries);
        Assert.Equal(2, fake.Calls);
    }

    [Fact]
    public async Task Collection_UnexpectedFailureIsSanitized()
    {
        var fake = new Persistence { Failure = "failure" };
        await using var factory = CreateFactory(fake);
        using var client = factory.CreateClient();
        await AssertProblem(await client.GetAsync(Route(fake.Team.Id)), HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Collection_AbortedRequestRemainsCancellation()
    {
        var fake = new Persistence { Failure = "cancel" };
        await using var factory = CreateFactory(fake);
        using var client = factory.CreateClient();
        var response = await client.GetAsync(Route(fake.Team.Id));
        Assert.Equal(499, (int)response.StatusCode);
        Assert.Equal(string.Empty, await response.Content.ReadAsStringAsync());
    }

    private static string Route(Guid teamId) => $"/api/teams/{teamId}/roster";

    private static Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program> CreateFactory(
        Persistence fake, string environment = "Development")
        => new ApiTestFactory(environment).WithWebHostBuilder(builder => builder.ConfigureServices(services =>
        {
            services.RemoveAll<IMembershipPersistence>();
            services.RemoveAll<IRosterPersistence>();
            services.RemoveAll<IRosterQueries>();
            services.RemoveAll<ITeamPersistence>();
            services.AddHttpContextAccessor();
            services.AddSingleton<IRosterPersistence>(provider =>
            {
                fake.ContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
                return fake;
            });
            services.AddSingleton<IRosterQueries>(provider =>
            {
                fake.ContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
                return fake;
            });
            services.AddSingleton<ITeamPersistence>(fake);
            services.AddSingleton<IMembershipPersistence>(fake);
        }));

    private static async Task AssertProblem(HttpResponseMessage response, HttpStatusCode expected)
    {
        Assert.Equal(expected, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var problem = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal((int)expected, problem.GetProperty("status").GetInt32());
        Assert.False(string.IsNullOrWhiteSpace(problem.GetProperty("title").GetString()));
        Assert.False(string.IsNullOrWhiteSpace(problem.GetProperty("type").GetString()));
        if (expected == HttpStatusCode.BadRequest) Assert.True(problem.GetProperty("errors").EnumerateObject().Any());
        Assert.DoesNotContain("private-provider-detail", problem.GetRawText());
        Assert.DoesNotContain("Exception", problem.GetRawText());
    }

    private sealed class Persistence : IRosterPersistence, IRosterQueries, IMembershipPersistence, ITeamPersistence
    {
        public Person Person { get; } = new(Guid.NewGuid(), "Alex", "Player");
        public Team Team { get; } = new(Guid.NewGuid(), "Local FC");
        public TeamMembership Membership { get; }
        public ConcurrentDictionary<Guid, RosterEntry> Entries { get; } = new();
        public int Calls { get; private set; }
        public string? Failure { get; init; }
        public IReadOnlyList<RosterDetails>? CollectionResults { get; init; }
        public IHttpContextAccessor? ContextAccessor { get; set; }

        public Persistence(TeamRole role = TeamRole.Player)
            => Membership = new TeamMembership(Guid.NewGuid(), Person.Id, Team.Id, role);

        private void Visit(CancellationToken token, bool fail = false)
        {
            Calls++;
            token.ThrowIfCancellationRequested();
            if (!fail) return;
            var inner = new InvalidOperationException("private-provider-detail");
            switch (Failure)
            {
                case "failure": throw inner;
                case "conflict": throw new RosterEntryConflictException(Guid.NewGuid(), Membership.Id, inner);
                case "reference": throw new RosterMembershipNotFoundException(Membership.Id, inner);
                case "cancel":
                    ContextAccessor!.HttpContext!.RequestAborted = new CancellationToken(canceled: true);
                    throw new OperationCanceledException(ContextAccessor.HttpContext.RequestAborted);
            }
        }

        public Task AddEntryAsync(RosterEntry entry, CancellationToken cancellationToken = default)
        {
            Visit(cancellationToken, fail: true);
            lock (Entries)
            {
                if (Entries.Values.Any(e => e.TeamMembershipId == entry.TeamMembershipId) || !Entries.TryAdd(entry.Id, entry))
                    throw new RosterEntryConflictException(entry.Id, entry.TeamMembershipId);
            }
            return Task.CompletedTask;
        }

        public Task<TeamMembership?> GetMembershipAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Visit(cancellationToken);
            return Task.FromResult(id == Membership.Id ? Membership : null);
        }

        public Task<RosterDetails?> GetByIdAsync(Guid teamId, Guid id, CancellationToken cancellationToken = default)
        {
            Visit(cancellationToken, fail: true);
            Entries.TryGetValue(id, out var entry);
            return Task.FromResult(entry is null || teamId != Team.Id || Membership.TeamRole != TeamRole.Player ? null
                : new RosterDetails(entry.Id, entry.TeamMembershipId,
                    new RosterPerson(Person.Id, Person.FirstName, Person.LastName), entry.JerseyNumber, entry.RosterStatus));
        }

        Task<MembershipDetails?> IMembershipPersistence.GetByIdAsync(Guid teamId, Guid id, CancellationToken token)
        {
            Visit(token);
            return Task.FromResult(teamId != Team.Id || id != Membership.Id ? null
                : new MembershipDetails(Membership.Id, Team.Id,
                    new MembershipPerson(Person.Id, Person.FirstName, Person.LastName), Membership.TeamRole));
        }

        public Task<Team?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Visit(cancellationToken);
            return Task.FromResult(id == Team.Id ? Team : null);
        }

        Task<IReadOnlyList<RosterDetails>> IRosterQueries.GetByTeamIdAsync(Guid teamId, CancellationToken cancellationToken)
        {
            Visit(cancellationToken, fail: true);
            IReadOnlyList<RosterDetails> result = teamId != Team.Id || Membership.TeamRole != TeamRole.Player ? []
                : CollectionResults ?? Entries.Values.Select(entry => new RosterDetails(entry.Id, entry.TeamMembershipId,
                    new RosterPerson(Person.Id, Person.FirstName, Person.LastName), entry.JerseyNumber, entry.RosterStatus)).ToArray();
            return Task.FromResult(result);
        }

        public Task AddAsync(Team team, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task AddAsync(TeamMembership membership, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyList<MembershipDetails>> GetByTeamIdAsync(Guid teamId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
