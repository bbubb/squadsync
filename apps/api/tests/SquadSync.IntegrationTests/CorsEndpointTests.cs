using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SquadSync.Application.Teams;
using Xunit;

namespace SquadSync.IntegrationTests;

public class CorsEndpointTests
{
    private const string WebOrigin = "http://127.0.0.1:3000";

    [Fact]
    public async Task TeamGet_ApprovedOrigin_CanReadResponseWithoutCredentials()
    {
        await using var factory = new ApiTestFactory().WithWebHostBuilder(builder =>
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<ITeamQueries>();
                services.AddSingleton<ITeamQueries, EmptyTeamQueries>();
            }));
        using var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/teams");
        request.Headers.Add("Origin", WebOrigin);
        using var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("[]", await response.Content.ReadAsStringAsync());
        Assert.Equal(WebOrigin, Assert.Single(response.Headers.GetValues("Access-Control-Allow-Origin")));
        Assert.False(response.Headers.Contains("Access-Control-Allow-Credentials"));
    }

    [Theory]
    [InlineData("http://localhost:3000")]
    [InlineData("http://127.0.0.1:3001")]
    [InlineData("https://127.0.0.1:3000")]
    [InlineData("http://192.168.1.10:3000")]
    [InlineData("https://example.com")]
    [InlineData("null")]
    public async Task UnapprovedOrigin_ReceivesNoCorsPermission(string origin)
    {
        await using var factory = new ApiTestFactory();
        using var client = factory.CreateClient();
        foreach (var method in new[] { HttpMethod.Get, HttpMethod.Options })
        {
            using var request = CreateRequest(method, "/api/teams/not-a-guid", origin);
            using var response = await client.SendAsync(request);
            AssertNoCorsPermission(response);
        }
        using var postPreflight = CreateRequest(HttpMethod.Options, "/api/teams", origin, "POST");
        postPreflight.Headers.Add("Access-Control-Request-Headers", "content-type");
        using var postResponse = await client.SendAsync(postPreflight);
        AssertNoCorsPermission(postResponse);
    }

    [Theory]
    [InlineData("/api/teams", "GET", "accept")]
    [InlineData("/api/people/11111111-1111-1111-1111-111111111111", "GET", "accept")]
    [InlineData("/api/teams/11111111-1111-1111-1111-111111111111/memberships", "GET", "accept")]
    [InlineData("/api/teams/11111111-1111-1111-1111-111111111111/roster", "GET", "accept")]
    [InlineData("/api/people/11111111-1111-1111-1111-111111111111/player-profile", "GET", "accept")]
    [InlineData("/api/teams", "POST", "content-type")]
    [InlineData("/api/teams", "POST", "accept,content-type")]
    [InlineData("/api/people", "POST", "content-type")]
    [InlineData("/api/teams/11111111-1111-1111-1111-111111111111/memberships", "POST", "content-type")]
    [InlineData("/api/teams/11111111-1111-1111-1111-111111111111/roster", "POST", "content-type")]
    [InlineData("/api/people/11111111-1111-1111-1111-111111111111/player-profile", "POST", "content-type")]
    public async Task ManagementPreflight_ApprovesOnlyGetPostAndRequiredHeaders(
        string path, string method, string headers)
    {
        await using var factory = new ApiTestFactory();
        using var client = factory.CreateClient();
        using var request = CreateRequest(HttpMethod.Options, path, WebOrigin, method);
        // JSON POST preflights name Content-Type; the application/json value belongs to the later POST.
        request.Headers.Add("Access-Control-Request-Headers", headers);
        using var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(WebOrigin, Assert.Single(response.Headers.GetValues("Access-Control-Allow-Origin")));
        AssertAllowedPolicy(response);
    }

    [Theory]
    [InlineData("PUT", null, HttpStatusCode.NoContent)]
    [InlineData("PUT", "content-type", HttpStatusCode.NoContent)]
    [InlineData("DELETE", null, HttpStatusCode.NotFound)]
    [InlineData("GET", "authorization", HttpStatusCode.NoContent)]
    [InlineData("GET", "x-custom-header", HttpStatusCode.NoContent)]
    [InlineData("POST", "authorization", HttpStatusCode.NoContent)]
    [InlineData("POST", "content-type,x-custom-header", HttpStatusCode.NoContent)]
    public async Task UnsupportedPreflight_DoesNotPermitRequestedMethodOrHeader(
        string method, string? header, HttpStatusCode expected)
    {
        await using var factory = new ApiTestFactory();
        using var client = factory.CreateClient();
        var path = method == "PUT"
            ? "/api/people/11111111-1111-1111-1111-111111111111/player-profile"
            : "/api/teams";
        using var request = CreateRequest(HttpMethod.Options, path, WebOrigin, method);
        if (header is not null) request.Headers.Add("Access-Control-Request-Headers", header);
        using var response = await client.SendAsync(request);

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.NotFound)
        {
            AssertNoCorsPermission(response);
            return;
        }
        // ASP.NET Core can return 204 plus the policy's allow-list even for a rejected request.
        // A browser must find its requested method and every requested header in that list.
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        AssertAllowedPolicy(response);
    }

    [Theory]
    [InlineData("Development", "/health", HttpStatusCode.OK)]
    [InlineData("Development", "/health/ready", HttpStatusCode.ServiceUnavailable)]
    [InlineData("Development", "/swagger/v1/swagger.json", HttpStatusCode.OK)]
    [InlineData("Staging", "/api/teams", HttpStatusCode.NotFound)]
    [InlineData("Production", "/api/teams", HttpStatusCode.NotFound)]
    [InlineData("Staging", "/api/people", HttpStatusCode.NotFound)]
    [InlineData("Production", "/api/people", HttpStatusCode.NotFound)]
    [InlineData("Staging", "/api/teams/11111111-1111-1111-1111-111111111111/memberships", HttpStatusCode.NotFound)]
    [InlineData("Production", "/api/teams/11111111-1111-1111-1111-111111111111/memberships", HttpStatusCode.NotFound)]
    [InlineData("Staging", "/api/teams/11111111-1111-1111-1111-111111111111/roster", HttpStatusCode.NotFound)]
    [InlineData("Production", "/api/teams/11111111-1111-1111-1111-111111111111/roster", HttpStatusCode.NotFound)]
    [InlineData("Staging", "/api/people/11111111-1111-1111-1111-111111111111/player-profile", HttpStatusCode.NotFound)]
    [InlineData("Production", "/api/people/11111111-1111-1111-1111-111111111111/player-profile", HttpStatusCode.NotFound)]
    public async Task OutsidePolicyScope_NoCorsHeadersOrPreflightPermission(
        string environment, string path, HttpStatusCode expected)
    {
        await using var factory = new ApiTestFactory(environment);
        using var client = factory.CreateClient();
        using var get = CreateRequest(HttpMethod.Get, path, WebOrigin);
        using var response = await client.SendAsync(get);
        Assert.Equal(expected, response.StatusCode);
        AssertNoCorsPermission(response);

        using var options = CreateRequest(HttpMethod.Options, path, WebOrigin);
        using var preflight = await client.SendAsync(options);
        AssertNoCorsPermission(preflight);
        if (expected == HttpStatusCode.NotFound) Assert.Equal(expected, preflight.StatusCode);

        using var postOptions = CreateRequest(HttpMethod.Options, path, WebOrigin, "POST");
        postOptions.Headers.Add("Access-Control-Request-Headers", "content-type");
        using var postPreflight = await client.SendAsync(postOptions);
        AssertNoCorsPermission(postPreflight);
        if (expected == HttpStatusCode.NotFound)
        {
            Assert.Equal(expected, postPreflight.StatusCode);
            using var post = CreateRequest(HttpMethod.Post, path, WebOrigin);
            post.Content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");
            using var postResponse = await client.SendAsync(post);
            Assert.Equal(HttpStatusCode.NotFound, postResponse.StatusCode);
            AssertNoCorsPermission(postResponse);
        }
    }

    private static HttpRequestMessage CreateRequest(HttpMethod method, string path, string origin, string requestedMethod = "GET")
    {
        var request = new HttpRequestMessage(method, path);
        request.Headers.Add("Origin", origin);
        if (method == HttpMethod.Options) request.Headers.Add("Access-Control-Request-Method", requestedMethod);
        return request;
    }

    private static void AssertAllowedPolicy(HttpResponseMessage response)
    {
        Assert.Equal(WebOrigin, Assert.Single(response.Headers.GetValues("Access-Control-Allow-Origin")));
        Assert.Equal(new[] { "GET", "POST" }, response.Headers.GetValues("Access-Control-Allow-Methods")
            .SelectMany(value => value.Split(',', StringSplitOptions.TrimEntries)));
        Assert.Equal(new[] { "Accept", "Content-Type" }, response.Headers.GetValues("Access-Control-Allow-Headers")
            .SelectMany(value => value.Split(',', StringSplitOptions.TrimEntries)));
        Assert.False(response.Headers.Contains("Access-Control-Allow-Credentials"));
    }

    private static void AssertNoCorsPermission(HttpResponseMessage response)
    {
        Assert.DoesNotContain(response.Headers, header => header.Key.StartsWith("Access-Control-Allow-", StringComparison.OrdinalIgnoreCase));
    }

    private sealed class EmptyTeamQueries : ITeamQueries
    {
        public Task<IReadOnlyList<TeamDetails>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<TeamDetails>>([]);
    }
}
