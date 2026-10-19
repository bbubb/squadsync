using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SquadSync.Api.PlayerProfiles;
using SquadSync.Application.People;
using SquadSync.Application.PlayerProfiles;
using SquadSync.Domain;
using Xunit;

namespace SquadSync.IntegrationTests;

public class PlayerProfilesEndpointTests
{
    [Theory]
    [InlineData("{}", null, null, null)]
    [InlineData("{\"dominantFoot\":null,\"heightInches\":null,\"weightPounds\":null}", null, null, null)]
    [InlineData("{\"dominantFoot\":\"Left\",\"heightInches\":70,\"weightPounds\":152.5123}", "Left", 70, 152.5123)]
    [InlineData("{\"dominantFoot\":\"Right\",\"weightPounds\":0.001}", "Right", null, 0.001)]
    [InlineData("{\"dominantFoot\":\"Both\"}", "Both", null, null)]
    public async Task ExplicitCreate_WithoutMembership_RoundTripsTypedProfileAndLocation(string payload, string? foot, int? height, double? weight)
    {
        var fake = new Persistence();
        await using var factory = CreateFactory(fake);
        using var client = factory.CreateClient();
        var created = await Post(client, Route(fake.Person.Id), payload);
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var profile = (await created.Content.ReadFromJsonAsync<PlayerProfileResponse>())!;
        Assert.NotEqual(Guid.Empty, profile.Id);
        Assert.Equal(new PlayerProfileResponse(profile.Id, fake.Person.Id, foot, height, (decimal?)weight), profile);
        Assert.Equal(Route(fake.Person.Id), created.Headers.Location!.AbsolutePath);
        var retrieved = await client.GetAsync(created.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, retrieved.StatusCode);
        var body = await retrieved.Content.ReadAsStringAsync();
        Assert.Equal(profile, JsonSerializer.Deserialize<PlayerProfileResponse>(body, JsonSerializerOptions.Web));
        var json = JsonSerializer.Deserialize<JsonElement>(body);
        Assert.Equal(new[] { "dominantFoot", "heightInches", "id", "personId", "weightPounds" },
            json.EnumerateObject().Select(property => property.Name).Order());
        await AssertProblem(await Post(client, Route(fake.Person.Id), "{}"), HttpStatusCode.Conflict);
        Assert.Single(fake.Profiles);
    }

    [Theory]
    [InlineData("")]
    [InlineData("null")]
    [InlineData("{")]
    [InlineData("{\"dominantFoot\":0}")]
    [InlineData("{\"dominantFoot\":\"0\"}")]
    [InlineData("{\"dominantFoot\":\"left\"}")]
    [InlineData("{\"dominantFoot\":\"Unknown\"}")]
    [InlineData("{\"dominantFoot\":\" Left\"}")]
    [InlineData("{\"dominantFoot\":\"Left, Right\"}")]
    [InlineData("{\"dominantFoot\":\"\"}")]
    [InlineData("{\"heightInches\":0}")]
    [InlineData("{\"heightInches\":-1}")]
    [InlineData("{\"heightInches\":70.5}")]
    [InlineData("{\"heightInches\":2147483648}")]
    [InlineData("{\"heightInches\":\"70\"}")]
    [InlineData("{\"weightPounds\":0}")]
    [InlineData("{\"weightPounds\":-0.01}")]
    [InlineData("{\"weightPounds\":\"152.5\"}")]
    [InlineData("{\"weightPounds\":true}")]
    [InlineData("{\"weightPounds\":1e100}")]
    [InlineData("{\"id\":\"00000000-0000-0000-0000-000000000001\"}")]
    [InlineData("{\"personId\":\"00000000-0000-0000-0000-000000000001\"}")]
    public async Task InvalidPayload_Returns400BeforePersistence(string payload)
    {
        var fake = new Persistence();
        await using var factory = CreateFactory(fake);
        using var client = factory.CreateClient();
        await AssertProblem(await Post(client, Route(fake.Person.Id), payload), HttpStatusCode.BadRequest);
        Assert.Equal(0, fake.Calls);
    }

    [Theory]
    [InlineData("not-a-guid")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public async Task InvalidRoute_Returns400BeforePersistence(string id)
    {
        var fake = new Persistence();
        await using var factory = CreateFactory(fake);
        using var client = factory.CreateClient();
        var route = $"/api/people/{id}/player-profile";
        await AssertProblem(await Post(client, route, "{}"), HttpStatusCode.BadRequest);
        await AssertProblem(await client.GetAsync(route), HttpStatusCode.BadRequest);
        Assert.Equal(0, fake.Calls);
    }

    [Fact]
    public async Task MissingPersonOrProfile_Returns404WithoutCreating()
    {
        var fake = new Persistence();
        await using var factory = CreateFactory(fake);
        using var client = factory.CreateClient();
        await AssertProblem(await Post(client, Route(Guid.NewGuid()), "{}"), HttpStatusCode.NotFound);
        await AssertProblem(await client.GetAsync(Route(Guid.NewGuid())), HttpStatusCode.NotFound);
        await AssertProblem(await client.GetAsync(Route(fake.Person.Id)), HttpStatusCode.NotFound);
        Assert.Empty(fake.Profiles);
    }

    [Fact]
    public async Task CompetingCreates_ProduceOneProfileAndOneConflict()
    {
        var fake = new Persistence { Compete = true };
        await using var factory = CreateFactory(fake);
        using var client = factory.CreateClient();
        var responses = await Task.WhenAll(Post(client, Route(fake.Person.Id), "{}"), Post(client, Route(fake.Person.Id), "{}"));
        Assert.Single(responses, response => response.StatusCode == HttpStatusCode.Created);
        await AssertProblem(Assert.Single(responses, response => response.StatusCode == HttpStatusCode.Conflict), HttpStatusCode.Conflict);
        Assert.Single(fake.Profiles);
    }

    [Theory]
    [InlineData("conflict", 409)]
    [InlineData("reference", 404)]
    [InlineData("failure", 500)]
    public async Task WriteFailures_AreSanitized(string failure, int status)
    {
        var fake = new Persistence { Failure = failure };
        await using var factory = CreateFactory(fake);
        using var client = factory.CreateClient();
        await AssertProblem(await Post(client, Route(fake.Person.Id), "{}"), (HttpStatusCode)status);
        Assert.Empty(fake.Profiles);
    }

    [Fact]
    public async Task ReadFailure_IsSanitized()
    {
        var fake = new Persistence { Failure = "read-failure" };
        await using var factory = CreateFactory(fake);
        using var client = factory.CreateClient();
        await AssertProblem(await client.GetAsync(Route(fake.Person.Id)), HttpStatusCode.InternalServerError);
    }

    [Theory]
    [InlineData(true, "cancel")]
    [InlineData(true, "cancel-write")]
    [InlineData(false, "cancel")]
    public async Task AbortedRequest_RemainsCancellation(bool create, string failure)
    {
        var fake = new Persistence { Failure = failure };
        await using var factory = CreateFactory(fake);
        using var client = factory.CreateClient();
        var response = create ? await Post(client, Route(fake.Person.Id), "{}") : await client.GetAsync(Route(fake.Person.Id));
        Assert.Equal(499, (int)response.StatusCode);
        Assert.Equal(string.Empty, await response.Content.ReadAsStringAsync());
        Assert.Empty(fake.Profiles);
    }

    [Theory]
    [InlineData("Production")]
    [InlineData("Staging")]
    public async Task Routes_AreAbsentOutsideDevelopment(string environment)
    {
        var fake = new Persistence();
        await using var factory = CreateFactory(fake, environment);
        using var client = factory.CreateClient();
        Assert.Equal(HttpStatusCode.NotFound, (await Post(client, Route(fake.Person.Id), "{}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync(Route(fake.Person.Id))).StatusCode);
        Assert.Equal(0, fake.Calls);
    }

    private static string Route(Guid personId) => $"/api/people/{personId}/player-profile";
    private static Task<HttpResponseMessage> Post(HttpClient client, string route, string payload)
        => client.PostAsync(route, new StringContent(payload, Encoding.UTF8, "application/json"));

    private static Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program> CreateFactory(Persistence fake, string environment = "Development")
        => new ApiTestFactory(environment).WithWebHostBuilder(builder => builder.ConfigureServices(services =>
        {
            services.RemoveAll<IPlayerProfilePersistence>();
            services.RemoveAll<IPersonPersistence>();
            services.AddHttpContextAccessor();
            services.AddSingleton<IPlayerProfilePersistence>(provider =>
            {
                fake.ContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
                return fake;
            });
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

    private sealed class Persistence : IPlayerProfilePersistence, IPersonPersistence
    {
        public Person Person { get; } = new(Guid.NewGuid(), "Alex", "Player");
        public ConcurrentDictionary<Guid, PlayerProfileDetails> Profiles { get; } = new();
        private int calls;
        public int Calls => calls;
        public string? Failure { get; init; }
        public bool Compete { get; init; }
        private int readers;
        private readonly TaskCompletionSource bothRead = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public IHttpContextAccessor? ContextAccessor { get; set; }

        private void Visit(CancellationToken token)
        {
            Interlocked.Increment(ref calls);
            token.ThrowIfCancellationRequested();
            if (Failure == "cancel")
            {
                ContextAccessor!.HttpContext!.RequestAborted = new CancellationToken(true);
                throw new OperationCanceledException(ContextAccessor.HttpContext.RequestAborted);
            }
        }

        public async Task<PlayerProfileDetails?> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default)
        {
            Visit(cancellationToken);
            if (Failure == "read-failure") throw new InvalidOperationException("private-provider-detail");
            Profiles.TryGetValue(personId, out var profile);
            if (Compete)
            {
                if (Interlocked.Increment(ref readers) == 2) bothRead.TrySetResult();
                await bothRead.Task.WaitAsync(TimeSpan.FromSeconds(10), cancellationToken);
            }
            return profile;
        }

        public Task AddAsync(PlayerProfile profile, CancellationToken cancellationToken = default)
        {
            Visit(cancellationToken);
            if (Failure == "cancel-write")
            {
                ContextAccessor!.HttpContext!.RequestAborted = new CancellationToken(true);
                throw new OperationCanceledException(ContextAccessor.HttpContext.RequestAborted);
            }
            var inner = new InvalidOperationException("private-provider-detail");
            switch (Failure)
            {
                case "conflict": throw new PlayerProfileConflictException(inner);
                case "reference": throw new PlayerProfilePersonNotFoundException(inner);
                case "failure": throw inner;
            }
            if (!Profiles.TryAdd(profile.PersonId, PlayerProfileDetails.From(profile))) throw new PlayerProfileConflictException(inner);
            return Task.CompletedTask;
        }

        public Task<Person?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Visit(cancellationToken);
            return Task.FromResult(id == Person.Id ? Person : null);
        }
        public Task AddAsync(Person person, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
