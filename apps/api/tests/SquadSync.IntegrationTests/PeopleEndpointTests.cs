using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SquadSync.Api.People;
using SquadSync.Application.People;
using SquadSync.Domain;
using Xunit;

namespace SquadSync.IntegrationTests;

public class PeopleEndpointTests
{
    [Fact]
    public async Task CreateAndGet_UseRealApplicationAndReturnMinimalNormalizedResource()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var created = await client.PostAsJsonAsync("/api/people", new { firstName = "  Alex ", lastName = " Player\t" });

        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var person = (await created.Content.ReadFromJsonAsync<PersonResponse>())!;
        Assert.NotEqual(Guid.Empty, person.Id);
        Assert.Equal("Alex", person.FirstName);
        Assert.Equal("Player", person.LastName);
        Assert.Equal($"/api/people/{person.Id}", created.Headers.Location!.AbsolutePath);

        var retrieved = await client.GetAsync(created.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, retrieved.StatusCode);
        var body = await retrieved.Content.ReadAsStringAsync();
        Assert.Equal(person, JsonSerializer.Deserialize<PersonResponse>(body, JsonSerializerOptions.Web));
        var json = JsonSerializer.Deserialize<JsonElement>(body);
        Assert.Equal(new[] { "firstName", "id", "lastName" }, json.EnumerateObject().Select(p => p.Name).Order().ToArray());
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("null")]
    [InlineData("")]
    [InlineData("{\"firstName\":")]
    [InlineData("{\"firstName\":42,\"lastName\":\"Player\"}")]
    [InlineData("{\"firstName\":null,\"lastName\":\"Player\"}")]
    [InlineData("{\"firstName\":\"\",\"lastName\":\"Player\"}")]
    [InlineData("{\"firstName\":\" \\t\",\"lastName\":\"Player\"}")]
    [InlineData("{\"firstName\":\"Alex\"}")]
    [InlineData("{\"firstName\":\"Alex\",\"lastName\":null}")]
    [InlineData("{\"firstName\":\"Alex\",\"lastName\":\"\"}")]
    [InlineData("{\"firstName\":\"Alex\",\"lastName\":\" \\t\"}")]
    public async Task InvalidCreate_ReturnsValidationProblemWithoutSaving(string payload)
    {
        var persistence = new TestPersonPersistence();
        await using var factory = CreateFactory(persistence: persistence);
        using var client = factory.CreateClient();

        var response = await client.PostAsync("/api/people", new StringContent(payload, Encoding.UTF8, "application/json"));

        var problem = await AssertProblem(response, HttpStatusCode.BadRequest);
        Assert.True(problem.GetProperty("errors").EnumerateObject().Any());
        Assert.Equal(0, persistence.Count);
    }

    [Fact]
    public async Task MissingPerson_ReturnsNotFoundProblem()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();
        await AssertProblem(await client.GetAsync($"/api/people/{Guid.NewGuid()}"), HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task MalformedId_ReturnsValidationProblem()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();
        var problem = await AssertProblem(await client.GetAsync("/api/people/not-a-guid"), HttpStatusCode.BadRequest);
        Assert.True(problem.GetProperty("errors").EnumerateObject().Any());
    }

    [Theory]
    [InlineData("Staging")]
    [InlineData("Production")]
    public async Task PeopleRoutes_AreUnavailableOutsideDevelopment(string environment)
    {
        var persistence = new TestPersonPersistence();
        await using var factory = CreateFactory(environment, persistence);
        using var client = factory.CreateClient();

        Assert.Equal(HttpStatusCode.NotFound,
            (await client.PostAsJsonAsync("/api/people", new { firstName = "Alex", lastName = "Player" })).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/people/{Guid.NewGuid()}")).StatusCode);
        Assert.Equal(0, persistence.Count);
    }

    private static Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program> CreateFactory(
        string environment = "Development", TestPersonPersistence? persistence = null)
        => new ApiTestFactory(environment).WithWebHostBuilder(builder => builder.ConfigureServices(services =>
        {
            services.RemoveAll<IPersonPersistence>();
            services.AddSingleton<IPersonPersistence>(persistence ?? new TestPersonPersistence());
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

    private sealed class TestPersonPersistence : IPersonPersistence
    {
        private readonly ConcurrentDictionary<Guid, Person> people = new();
        public int Count => people.Count;

        public Task AddAsync(Person person, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Assert.True(people.TryAdd(person.Id, person));
            return Task.CompletedTask;
        }

        public Task<Person?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            people.TryGetValue(id, out var person);
            return Task.FromResult(person);
        }
    }
}
