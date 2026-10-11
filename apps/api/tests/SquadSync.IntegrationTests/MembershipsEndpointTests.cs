using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SquadSync.Api.Memberships;
using SquadSync.Application.Memberships;
using SquadSync.Application.People;
using SquadSync.Application.Teams;
using SquadSync.Domain;
using Xunit;

namespace SquadSync.IntegrationTests;

public class MembershipsEndpointTests
{
    [Theory]
    [InlineData("Owner")]
    [InlineData("Coach")]
    [InlineData("AssistantCoach")]
    [InlineData("Manager")]
    [InlineData("Player")]
    [InlineData("Viewer")]
    public async Task Create_LocationReturnsSameMembershipAndPersonSummary(string role)
    {
        var fake = new Persistence();
        await using var factory = CreateFactory(fake);
        using var client = factory.CreateClient();
        var created = await client.PostAsJsonAsync(Route(fake.Team.Id), new { personId = fake.Person.Id, teamRole = role });
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var membership = (await created.Content.ReadFromJsonAsync<MembershipResponse>())!;
        Assert.NotEqual(Guid.Empty, membership.Id);
        Assert.Equal(fake.Team.Id, membership.TeamId);
        Assert.Equal(new MembershipPersonResponse(fake.Person.Id, "Alex", "Player"), membership.Person);
        Assert.Equal(role, membership.TeamRole);
        Assert.Equal($"{Route(fake.Team.Id)}/{membership.Id}", created.Headers.Location!.AbsolutePath);
        var retrieved = await client.GetAsync(created.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, retrieved.StatusCode);
        var body = await retrieved.Content.ReadAsStringAsync();
        Assert.Equal(membership, JsonSerializer.Deserialize<MembershipResponse>(body, JsonSerializerOptions.Web));
        var json = JsonSerializer.Deserialize<JsonElement>(body);
        Assert.Equal(new[] { "id", "person", "teamId", "teamRole" }, json.EnumerateObject().Select(p => p.Name).Order());
        Assert.Equal(new[] { "firstName", "id", "lastName" }, json.GetProperty("person").EnumerateObject().Select(p => p.Name).Order());
        Assert.Single(fake.Memberships);

        await AssertProblem(await client.GetAsync($"{Route(Guid.NewGuid())}/{membership.Id}"), HttpStatusCode.NotFound);
        await AssertProblem(await client.PostAsJsonAsync(Route(fake.Team.Id), new { personId = fake.Person.Id, teamRole = "Coach" }), HttpStatusCode.Conflict);
        Assert.Single(fake.Memberships);
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("null")]
    [InlineData("")]
    [InlineData("{\"teamRole\":")]
    [InlineData("{\"personId\":\"PERSON\"}")]
    [InlineData("{\"personId\":\"PERSON\",\"teamRole\":null}")]
    [InlineData("{\"personId\":\"PERSON\",\"teamRole\":0}")]
    [InlineData("{\"personId\":\"PERSON\",\"teamRole\":\"0\"}")]
    [InlineData("{\"personId\":\"PERSON\",\"teamRole\":\"player\"}")]
    [InlineData("{\"personId\":\"PERSON\",\"teamRole\":\"Unknown\"}")]
    [InlineData("{\"personId\":\"PERSON\",\"teamRole\":\" Player\"}")]
    [InlineData("{\"personId\":\"PERSON\",\"teamRole\":\"Player\\n\"}")]
    [InlineData("{\"personId\":\"PERSON\",\"teamRole\":\"Owner, Player\"}")]
    [InlineData("{\"personId\":\"PERSON\",\"teamRole\":\"\"}")]
    [InlineData("{\"personId\":null,\"teamRole\":\"Player\"}")]
    [InlineData("{\"teamRole\":\"Player\"}")]
    [InlineData("{\"personId\":\"not-a-guid\",\"teamRole\":\"Player\"}")]
    [InlineData("{\"personId\":\"00000000-0000-0000-0000-000000000000\",\"teamRole\":\"Player\"}")]
    public async Task InvalidPayload_ReturnsValidationProblemBeforePersistence(string payload)
    {
        var fake = new Persistence();
        await using var factory = CreateFactory(fake);
        using var client = factory.CreateClient();
        var response = await client.PostAsync(Route(fake.Team.Id),
            new StringContent(payload.Replace("PERSON", fake.Person.Id.ToString()), Encoding.UTF8, "application/json"));
        await AssertProblem(response, HttpStatusCode.BadRequest);
        Assert.Empty(fake.Memberships);
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
        await AssertProblem(await client.PostAsJsonAsync($"/api/teams/{id}/memberships",
            new { personId = fake.Person.Id, teamRole = "Player" }), HttpStatusCode.BadRequest);
        await AssertProblem(await client.GetAsync($"/api/teams/{id}/memberships/{Guid.NewGuid()}"), HttpStatusCode.BadRequest);
        await AssertProblem(await client.GetAsync($"{Route(fake.Team.Id)}/{id}"), HttpStatusCode.BadRequest);
        Assert.Equal(0, fake.Calls);
    }

    [Fact]
    public async Task MissingReferencesAndMembership_Return404WithoutSaving()
    {
        var fake = new Persistence();
        await using var factory = CreateFactory(fake);
        using var client = factory.CreateClient();
        await AssertProblem(await client.PostAsJsonAsync(Route(Guid.NewGuid()),
            new { personId = fake.Person.Id, teamRole = "Player" }), HttpStatusCode.NotFound);
        await AssertProblem(await client.PostAsJsonAsync(Route(fake.Team.Id),
            new { personId = Guid.NewGuid(), teamRole = "Player" }), HttpStatusCode.NotFound);
        await AssertProblem(await client.GetAsync($"{Route(fake.Team.Id)}/{Guid.NewGuid()}"), HttpStatusCode.NotFound);
        Assert.Empty(fake.Memberships);
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
            new { personId = fake.Person.Id, teamRole = "Player" }), (HttpStatusCode)status);
        Assert.Empty(fake.Memberships);
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
    public async Task AbortedMembershipRequest_RemainsCancellation(bool create)
    {
        var fake = new Persistence { Failure = "cancel" };
        await using var factory = CreateFactory(fake);
        using var client = factory.CreateClient();
        var response = create
            ? await client.PostAsJsonAsync(Route(fake.Team.Id), new { personId = fake.Person.Id, teamRole = "Player" })
            : await client.GetAsync($"{Route(fake.Team.Id)}/{Guid.NewGuid()}");
        Assert.Equal(499, (int)response.StatusCode);
        Assert.Equal(string.Empty, await response.Content.ReadAsStringAsync());
        Assert.Empty(fake.Memberships);
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
            new { personId = fake.Person.Id, teamRole = "Player" })).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"{Route(fake.Team.Id)}/{Guid.NewGuid()}")).StatusCode);
        Assert.Equal(0, fake.Calls);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/health")).StatusCode);
    }

    private static string Route(Guid teamId) => $"/api/teams/{teamId}/memberships";

    private static Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program> CreateFactory(
        Persistence fake, string environment = "Development")
        => new ApiTestFactory(environment).WithWebHostBuilder(builder => builder.ConfigureServices(services =>
        {
            services.RemoveAll<IMembershipPersistence>();
            services.RemoveAll<ITeamPersistence>();
            services.RemoveAll<IPersonPersistence>();
            services.AddHttpContextAccessor();
            services.AddSingleton<IMembershipPersistence>(provider =>
            {
                fake.ContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
                return fake;
            });
            services.AddSingleton<ITeamPersistence>(fake);
            services.AddSingleton<IPersonPersistence>(fake);
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

    private sealed class Persistence : IMembershipPersistence, IPersonPersistence, ITeamPersistence
    {
        public Person Person { get; } = new(Guid.NewGuid(), "Alex", "Player");
        public Team Team { get; } = new(Guid.NewGuid(), "Local FC");
        public ConcurrentDictionary<Guid, TeamMembership> Memberships { get; } = new();
        public int Calls { get; private set; }
        public string? Failure { get; init; }
        public IHttpContextAccessor? ContextAccessor { get; set; }

        private void Visit(CancellationToken token, bool fail = false)
        {
            Calls++;
            token.ThrowIfCancellationRequested();
            if (!fail) return;
            var inner = new InvalidOperationException("private-provider-detail");
            switch (Failure)
            {
                case "failure": throw inner;
                case "conflict": throw new MembershipConflictException(inner);
                case "reference": throw new MembershipReferenceNotFoundException(inner);
                case "cancel":
                    // Simulate persistence observing an aborted real-controller request.
                    ContextAccessor!.HttpContext!.RequestAborted = new CancellationToken(canceled: true);
                    throw new OperationCanceledException(ContextAccessor.HttpContext.RequestAborted);
            }
        }

        public Task AddAsync(TeamMembership membership, CancellationToken cancellationToken = default)
        {
            Visit(cancellationToken, fail: true);
            if (Memberships.Values.Any(entry => entry.PersonId == membership.PersonId && entry.TeamId == membership.TeamId))
                throw new MembershipConflictException();
            Assert.True(Memberships.TryAdd(membership.Id, membership));
            return Task.CompletedTask;
        }

        public Task<MembershipDetails?> GetByIdAsync(Guid teamId, Guid membershipId, CancellationToken cancellationToken = default)
        {
            Visit(cancellationToken, fail: true);
            Memberships.TryGetValue(membershipId, out var membership);
            return Task.FromResult(membership is null || membership.TeamId != teamId ? null
                : new MembershipDetails(membership.Id, membership.TeamId,
                    new MembershipPerson(Person.Id, Person.FirstName, Person.LastName), membership.TeamRole));
        }

        Task<Person?> IPersonPersistence.GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            Visit(cancellationToken);
            return Task.FromResult(id == Person.Id ? Person : null);
        }

        Task<Team?> ITeamPersistence.GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            Visit(cancellationToken);
            return Task.FromResult(id == Team.Id ? Team : null);
        }

        public Task AddAsync(Person person, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task AddAsync(Team team, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
