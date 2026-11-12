using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SquadSync.Application.People;
using Xunit;

namespace SquadSync.IntegrationTests;

public class PersonCandidateEndpointTests
{
    [Theory]
    [InlineData("")]
    [InlineData("?q=")]
    [InlineData("?q=%20%20")]
    [InlineData("?q=a")]
    [InlineData("?q=Alex&limit=0")]
    [InlineData("?q=Alex&limit=21")]
    [InlineData("?q=Alex&limit=no")]
    [InlineData("?q=Alex&limit=2147483648")]
    [InlineData("?q=Alex&cursor=invalid")]
    [InlineData("?q=Alex&cursor=")]
    [InlineData("?q=Alex&cursor=%20")]
    public async Task InvalidRequest_ReturnsSafeValidationProblemBeforeQuery(string query)
    {
        var fake = new Queries();
        await using var factory = CreateFactory(fake);
        using var client = factory.CreateClient();
        await AssertInvalid(client, query);
        Assert.Equal(0, fake.Calls);
    }

    [Fact]
    public async Task OverlongInputs_Return400BeforeQuery()
    {
        var fake = new Queries();
        await using var factory = CreateFactory(fake);
        using var client = factory.CreateClient();
        await AssertInvalid(client, "?q=" + new string('x', 101));
        await AssertInvalid(client, "?q=Alex&cursor=" + new string('a', 4097));
        Assert.Equal(0, fake.Calls);
    }

    [Fact]
    public async Task LiteralRoute_ReturnsMinimalPageAndRoundTripsCursor()
    {
        var fake = new Queries();
        var first = new PersonCandidate(Guid.NewGuid(), "Alex", "Player");
        var second = new PersonCandidate(Guid.NewGuid(), "Alex", "Player");
        fake.Rows = [new(first, new("player", "alex", first.PersonId)), new(second, new("player", "alex", second.PersonId))];
        await using var factory = CreateFactory(fake);
        using var client = factory.CreateClient();
        var response = await client.GetAsync("/api/people/candidates?q=%20Alex%20&limit=1");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(new[] { "items", "nextCursor" }, json.EnumerateObject().Select(p => p.Name).Order());
        Assert.Equal(new[] { "firstName", "lastName", "personId" }, json.GetProperty("items")[0].EnumerateObject().Select(p => p.Name).Order());
        var cursor = json.GetProperty("nextCursor").GetString();
        Assert.NotNull(cursor);
        Assert.Equal("Alex", fake.Query);
        Assert.Equal(2, fake.Take);
        await AssertInvalid(client, "?q=Other&cursor=" + cursor);
        fake.Rows = [fake.Rows[1]];
        var last = (await client.GetFromJsonAsync<PersonCandidatePage>("/api/people/candidates?q=Alex&limit=1&cursor=" + cursor))!;
        Assert.Equal(first.PersonId, fake.After!.PersonId);
        Assert.Equal(second, Assert.Single(last.Items));
        Assert.Null(last.NextCursor);
        fake.Rows = [];
        var empty = (await client.GetFromJsonAsync<PersonCandidatePage>("/api/people/candidates?q=Nobody"))!;
        Assert.Empty(empty.Items);
        Assert.Null(empty.NextCursor);
        Assert.Equal(21, fake.Take);
    }

    [Theory]
    [InlineData("Staging")]
    [InlineData("Production")]
    public async Task CandidateRoute_IsUnavailableOutsideDevelopment(string environment)
    {
        var fake = new Queries();
        await using var factory = CreateFactory(fake, environment);
        using var client = factory.CreateClient();
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/api/people/candidates?q=Alex")).StatusCode);
        Assert.Equal(0, fake.Calls);
    }

    [Fact]
    public async Task CandidateCors_AllowsOnlyExactDevelopmentOrigin()
    {
        await using var factory = CreateFactory(new Queries());
        using var client = factory.CreateClient();
        foreach (var origin in new[] { "http://127.0.0.1:3000", "http://localhost:3000", "https://example.com" })
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/people/candidates?q=Alex");
            request.Headers.Add("Origin", origin);
            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            if (origin == "http://127.0.0.1:3000")
                Assert.Equal(origin, Assert.Single(response.Headers.GetValues("Access-Control-Allow-Origin")));
            else
                Assert.False(response.Headers.Contains("Access-Control-Allow-Origin"));
        }
    }

    [Fact]
    public async Task Swagger_DescribesPageMinimalItemsAndValidation()
    {
        await using var factory = CreateFactory(new Queries());
        using var client = factory.CreateClient();
        var document = await client.GetFromJsonAsync<JsonElement>("/swagger/v1/swagger.json");
        var operation = document.GetProperty("paths").GetProperty("/api/people/candidates").GetProperty("get");
        var parameters = operation.GetProperty("parameters").EnumerateArray().ToArray();
        Assert.Equal(new[] { "cursor", "limit", "q" }, parameters.Select(parameter => parameter.GetProperty("name").GetString()).Order());
        Assert.True(parameters.Single(parameter => parameter.GetProperty("name").GetString() == "q").GetProperty("required").GetBoolean());
        Assert.Equal("#/components/schemas/PersonCandidatePageResponse", operation.GetProperty("responses").GetProperty("200")
            .GetProperty("content").GetProperty("application/json").GetProperty("schema").GetProperty("$ref").GetString());
        Assert.True(operation.GetProperty("responses").TryGetProperty("400", out _));
        var schemas = document.GetProperty("components").GetProperty("schemas");
        Assert.Equal(new[] { "firstName", "lastName", "personId" }, schemas.GetProperty("PersonCandidateResponse")
            .GetProperty("properties").EnumerateObject().Select(p => p.Name).Order());
        Assert.Equal(new[] { "firstName", "lastName", "personId" }, schemas.GetProperty("PersonCandidateResponse")
            .GetProperty("required").EnumerateArray().Select(p => p.GetString()).Order());
        var page = schemas.GetProperty("PersonCandidatePageResponse");
        Assert.Equal(new[] { "items", "nextCursor" }, page.GetProperty("required").EnumerateArray().Select(p => p.GetString()).Order());
        Assert.True(page.GetProperty("properties").GetProperty("nextCursor").GetProperty("nullable").GetBoolean());
        Assert.Equal("#/components/schemas/PersonCandidateResponse", page.GetProperty("properties").GetProperty("items")
            .GetProperty("items").GetProperty("$ref").GetString());
    }

    private static async Task AssertInvalid(HttpClient client, string query)
    {
        var response = await client.GetAsync("/api/people/candidates" + query);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(json.GetProperty("errors").EnumerateObject().Any());
        Assert.DoesNotContain("Position", json.ToString());
    }

    private static Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program> CreateFactory(Queries fake, string environment = "Development")
        => new ApiTestFactory(environment).WithWebHostBuilder(builder => builder.ConfigureServices(services =>
        {
            services.RemoveAll<IPersonCandidateQueries>();
            services.AddSingleton<IPersonCandidateQueries>(fake);
        }));

    private sealed class Queries : IPersonCandidateQueries
    {
        public IReadOnlyList<PersonCandidateRow> Rows { get; set; } = [];
        public int Calls { get; private set; }
        public int Take { get; private set; }
        public string? Query { get; private set; }
        public PersonCandidatePosition? After { get; private set; }
        public Task<IReadOnlyList<PersonCandidateRow>> SearchAsync(string query, int take, PersonCandidatePosition? after, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Calls++;
            Query = query;
            Take = take;
            After = after;
            return Task.FromResult(Rows);
        }
    }
}
