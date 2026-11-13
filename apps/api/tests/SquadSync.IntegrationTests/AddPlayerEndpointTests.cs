using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SquadSync.Api.Rosters;
using SquadSync.Application.Rosters;
using SquadSync.Domain;
using Xunit;

namespace SquadSync.IntegrationTests;

public sealed class AddPlayerEndpointTests
{
    private static string Route(Guid team) => $"/api/teams/{team}/roster/players";

    [Theory]
    [InlineData(false, "Active")]
    [InlineData(true, "Reserved")]
    [InlineData(true, "Suspended")]
    public async Task BothVariants_ReturnTyped201AndRetrievableLocation(bool existing, string status)
    {
        var store = new Store();
        await using var factory = Factory(store);
        using var client = factory.CreateClient();
        object body = existing ? new { personId = store.Person.Id, jerseyNumber = "007", rosterStatus = status }
            : new { newPerson = new { firstName = " Alex ", lastName = " Player " }, jerseyNumber = "007", rosterStatus = status };
        client.DefaultRequestHeaders.Add("Origin", "http://127.0.0.1:3000");
        var response = await client.PostAsJsonAsync(Route(store.Team), body);
        Assert.Equal("http://127.0.0.1:3000", Assert.Single(response.Headers.GetValues("Access-Control-Allow-Origin")));
        Assert.False(response.Headers.Contains("Access-Control-Allow-Credentials"));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = (await response.Content.ReadFromJsonAsync<RosterEntryResponse>())!;
        Assert.Equal("007", result.JerseyNumber);
        Assert.Equal("Alex", result.Person.FirstName);
        Assert.Equal(status, result.RosterStatus);
        Assert.Equal($"/api/teams/{store.Team}/roster/{result.Id}", response.Headers.Location!.AbsolutePath);
        Assert.Equal(result, await client.GetFromJsonAsync<RosterEntryResponse>(response.Headers.Location));
        Assert.Equal(existing, store.CreatedPerson is null);
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("null")]
    [InlineData("{")]
    [InlineData("{\"personId\":\"bad\",\"rosterStatus\":\"Active\"}")]
    [InlineData("{\"personId\":\"00000000-0000-0000-0000-000000000000\",\"rosterStatus\":\"Active\"}")]
    [InlineData("{\"rosterStatus\":\"Active\"}")]
    [InlineData("{\"personId\":\"PERSON\",\"newPerson\":{\"firstName\":\"A\",\"lastName\":\"B\"},\"rosterStatus\":\"Active\"}")]
    [InlineData("{\"newPerson\":{},\"rosterStatus\":\"Active\"}")]
    [InlineData("{\"newPerson\":{\"firstName\":\" \",\"lastName\":\"B\"},\"rosterStatus\":\"Active\"}")]
    [InlineData("{\"newPerson\":{\"firstName\":1,\"lastName\":\"B\"},\"rosterStatus\":\"Active\"}")]
    [InlineData("{\"personId\":\"PERSON\"}")]
    [InlineData("{\"personId\":\"PERSON\",\"rosterStatus\":0}")]
    [InlineData("{\"personId\":\"PERSON\",\"rosterStatus\":\"active\"}")]
    [InlineData("{\"personId\":\"PERSON\",\"rosterStatus\":\"Active\\n\"}")]
    [InlineData("{\"personId\":\"PERSON\",\"rosterStatus\":\"Active\",\"jerseyNumber\":7}")]
    [InlineData("{\"personId\":\"PERSON\",\"rosterStatus\":\"Active\",\"jerseyNumber\":\"1234\"}")]
    [InlineData("{\"personId\":\"PERSON\",\"rosterStatus\":\"Active\",\"jerseyNumber\":\"１２\"}")]
    public async Task InvalidBody_Returns400BeforePersistence(string payload)
    {
        var store = new Store();
        await using var factory = Factory(store);
        using var client = factory.CreateClient();
        await Problem(await client.PostAsync(Route(store.Team), new StringContent(
            payload.Replace("PERSON", store.Person.Id.ToString()), Encoding.UTF8, "application/json")), 400);
        Assert.Equal(0, store.Begins);
    }

    [Theory]
    [InlineData("bad")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public async Task InvalidRoute_Returns400BeforePersistence(string team)
    {
        var store = new Store();
        await using var factory = Factory(store);
        using var client = factory.CreateClient();
        await Problem(await client.PostAsJsonAsync($"/api/teams/{team}/roster/players",
            new { personId = store.Person.Id, rosterStatus = "Active" }), 400);
        Assert.Equal(0, store.Begins);
    }

    [Theory]
    [InlineData("team", 404)]
    [InlineData("person", 404)]
    [InlineData("role", 409)]
    [InlineData("conflict", 409)]
    [InlineData("failure", 500)]
    public async Task ReferencesConflictsAndUnexpectedFailures_AreSafe(string failure, int status)
    {
        var store = new Store { Failure = failure };
        await using var factory = Factory(store);
        using var client = factory.CreateClient();
        await Problem(await client.PostAsJsonAsync(Route(store.Team),
            new { personId = store.Person.Id, rosterStatus = "Active" }), status);
        Assert.Null(store.Result);
        Assert.Equal(1, store.Begins);
    }

    [Theory]
    [InlineData("Staging")]
    [InlineData("Production")]
    public async Task OutsideDevelopment_PostAndOptionsAreUnmapped(string environment)
    {
        var store = new Store();
        await using var factory = Factory(store, environment);
        using var client = factory.CreateClient();
        using var post = new HttpRequestMessage(HttpMethod.Post, Route(store.Team))
        { Content = JsonContent.Create(new { personId = store.Person.Id, rosterStatus = "Active" }) };
        post.Headers.Add("Origin", "http://127.0.0.1:3000");
        var response = await client.SendAsync(post);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False(response.Headers.Contains("Access-Control-Allow-Origin"));
        using var options = new HttpRequestMessage(HttpMethod.Options, Route(store.Team));
        options.Headers.Add("Origin", "http://127.0.0.1:3000");
        options.Headers.Add("Access-Control-Request-Method", "POST");
        options.Headers.Add("Access-Control-Request-Headers", "content-type");
        Assert.Equal(HttpStatusCode.NotFound, (await client.SendAsync(options)).StatusCode);
        Assert.Equal(0, store.Begins);
    }

    [Fact]
    public async Task AbortedRequest_RemainsCancellation()
    {
        var store = new Store { Failure = "cancel" };
        await using var factory = Factory(store);
        using var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync(Route(store.Team), new { personId = store.Person.Id, rosterStatus = "Active" });
        Assert.Equal(499, (int)response.StatusCode);
        Assert.Equal(string.Empty, await response.Content.ReadAsStringAsync());
        Assert.Null(store.Result);
        Assert.Equal(1, store.Begins);
    }

    [Fact]
    public async Task Swagger_DescribesAddPlayerRequestAndTypedResponses()
    {
        await using var factory = new ApiTestFactory();
        using var client = factory.CreateClient();
        var json = await client.GetFromJsonAsync<JsonElement>("/swagger/v1/swagger.json");
        var post = json.GetProperty("paths").GetProperty("/api/teams/{teamId}/roster/players").GetProperty("post");
        Assert.Equal(new[] { "201", "400", "404", "409" }, post.GetProperty("responses").EnumerateObject().Select(x => x.Name).Order());
        Assert.Equal("#/components/schemas/RosterEntryResponse", post.GetProperty("responses").GetProperty("201")
            .GetProperty("content").GetProperty("application/json").GetProperty("schema").GetProperty("$ref").GetString());
        var request = json.GetProperty("components").GetProperty("schemas").GetProperty("AddPlayerRequest");
        Assert.Equal(new[] { "jerseyNumber", "newPerson", "personId", "rosterStatus" }, request.GetProperty("properties").EnumerateObject().Select(x => x.Name).Order());
        Assert.Contains("rosterStatus", request.GetProperty("required").EnumerateArray().Select(x => x.GetString()));
        Assert.Contains("exactly one", request.GetProperty("description").GetString());
        Assert.Contains("ASCII digits", request.GetProperty("properties").GetProperty("jerseyNumber").GetProperty("description").GetString());
    }

    private static Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program> Factory(Store store, string environment = "Development")
        => new ApiTestFactory(environment).WithWebHostBuilder(builder => builder.ConfigureServices(services =>
        {
            services.RemoveAll<IAddPlayerPersistence>(); services.RemoveAll<IRosterQueries>();
            services.AddHttpContextAccessor();
            services.AddSingleton<IAddPlayerPersistence>(provider =>
            {
                store.Context = provider.GetRequiredService<IHttpContextAccessor>();
                return store;
            });
            services.AddSingleton<IRosterQueries>(store);
        }));

    private static async Task Problem(HttpResponseMessage response, int status)
    {
        Assert.Equal(status, (int)response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType!.MediaType);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(status, body.GetProperty("status").GetInt32());
        if (status == 400) Assert.True(body.GetProperty("errors").EnumerateObject().Any());
        Assert.DoesNotContain("private-detail", body.GetRawText());
    }

    private sealed class Store : IAddPlayerPersistence, IAddPlayerTransaction, IRosterQueries
    {
        public Guid Team = Guid.NewGuid();
        public Person Person = new(Guid.NewGuid(), "Alex", "Player");
        public Person? CreatedPerson;
        public RosterDetails? Result;
        public string? Failure;
        public int Begins;
        public IHttpContextAccessor? Context;
        public Task<IAddPlayerTransaction> BeginAsync(CancellationToken cancellationToken) { Begins++; return Task.FromResult<IAddPlayerTransaction>(this); }
        public Task<bool> TeamExistsAsync(Guid team, CancellationToken token) => Task.FromResult(Failure != "team");
        public Task<Person?> GetPersonAsync(Guid person, CancellationToken token) => Task.FromResult(Failure == "person" ? null : Person);
        public Task<TeamMembership?> GetMembershipAsync(Guid team, Guid person, CancellationToken token)
            => Task.FromResult(Failure == "role" ? new TeamMembership(Guid.NewGuid(), person, team, TeamRole.Coach) : null);
        public Task SaveAsync(Person? person, TeamMembership? membership, RosterEntry entry, CancellationToken token)
        {
            if (Failure == "conflict") throw new AddPlayerConflictException(new Exception("private-detail"));
            if (Failure == "failure") throw new InvalidOperationException("private-detail");
            if (Failure == "cancel")
            {
                Context!.HttpContext!.RequestAborted = new CancellationToken(true);
                throw new OperationCanceledException(Context.HttpContext.RequestAborted);
            }
            CreatedPerson = person;
            var selected = person ?? Person;
            Result = new(entry.Id, entry.TeamMembershipId, new(selected.Id, selected.FirstName, selected.LastName), entry.JerseyNumber, entry.RosterStatus);
            return Task.CompletedTask;
        }
        public Task CommitAsync(CancellationToken token) => Task.CompletedTask;
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
        public Task<RosterDetails?> GetByIdAsync(Guid team, Guid entry, CancellationToken token) => Task.FromResult(Result);
        public Task<IReadOnlyList<RosterDetails>> GetByTeamIdAsync(Guid team, CancellationToken token) => throw new NotSupportedException();
    }
}
