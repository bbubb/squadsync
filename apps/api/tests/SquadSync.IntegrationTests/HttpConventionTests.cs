using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xunit;

namespace SquadSync.IntegrationTests;

public class HttpConventionTests
{
    [Theory]
    [InlineData("Development", HttpStatusCode.OK)]
    [InlineData("Staging", HttpStatusCode.NotFound)]
    [InlineData("Production", HttpStatusCode.NotFound)]
    public async Task ManagementRoute_IsAvailableOnlyInDevelopment(string environment, HttpStatusCode expected)
    {
        await using var factory = new ApiTestFactory(environment, includeProbe: true);
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/management-probe", new { Name = "Valid probe" });

        Assert.Equal(expected, response.StatusCode);
        if (environment == "Development")
        {
            Assert.Equal("Valid probe", (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("name").GetString());
        }
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/health")).StatusCode);
        Assert.Equal(HttpStatusCode.ServiceUnavailable, (await client.GetAsync("/health/ready")).StatusCode);
        Assert.Equal(environment == "Development" ? HttpStatusCode.OK : HttpStatusCode.NotFound,
            (await client.GetAsync("/swagger/index.html")).StatusCode);
    }

    [Fact]
    public async Task RealApi_DoesNotContainProbeRoutes()
    {
        await using var factory = new ApiTestFactory();
        using var client = factory.CreateClient();

        Assert.Equal(HttpStatusCode.NotFound,
            (await client.PostAsJsonAsync("/api/management-probe", new { Name = "Valid probe" })).StatusCode);
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"name\":\"\"}")]
    [InlineData("{\"name\":")]
    [InlineData("{\"name\":42}")]
    public async Task InvalidRequest_ReturnsStandardValidationProblem(string payload)
    {
        await using var factory = new ApiTestFactory(includeProbe: true);
        using var client = factory.CreateClient();

        var response = await client.PostAsync("/api/management-probe", new StringContent(payload, Encoding.UTF8, "application/json"));

        var problem = await AssertProblem(response, HttpStatusCode.BadRequest);
        Assert.True(problem.GetProperty("errors").EnumerateObject().Any());
        Assert.Equal("One or more validation errors occurred.", problem.GetProperty("title").GetString());
    }

    [Fact]
    public async Task MissingResource_ReturnsNotFoundProblem()
    {
        await using var factory = new ApiTestFactory(includeProbe: true);
        using var client = factory.CreateClient();

        var problem = await AssertProblem(await client.GetAsync("/api/management-probe/missing"), HttpStatusCode.NotFound);

        Assert.Equal("Not Found", problem.GetProperty("title").GetString());
    }

    [Fact]
    public async Task UnexpectedError_ReturnsSafeInternalServerErrorProblem()
    {
        await using var factory = new ApiTestFactory(includeProbe: true);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/management-probe/throw");
        var problem = await AssertProblem(response, HttpStatusCode.InternalServerError);
        var body = problem.GetRawText();

        Assert.DoesNotContain("secret-probe-password", body);
        Assert.DoesNotContain("private-db", body);
        Assert.DoesNotContain("InvalidOperationException", body);
        Assert.DoesNotContain("ManagementProbeController", body);
        Assert.False(problem.TryGetProperty("detail", out _));
        Assert.False(problem.TryGetProperty("exception", out _));
    }

    [Fact]
    public async Task AbortedRequest_IsNotConvertedToInternalServerError()
    {
        await using var factory = new ApiTestFactory(includeProbe: true);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/management-probe/cancel");

        Assert.Equal(499, (int)response.StatusCode);
        Assert.Equal(string.Empty, await response.Content.ReadAsStringAsync());
    }

    private static async Task<JsonElement> AssertProblem(HttpResponseMessage response, HttpStatusCode status)
    {
        Assert.Equal(status, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var problem = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal((int)status, problem.GetProperty("status").GetInt32());
        Assert.False(string.IsNullOrWhiteSpace(problem.GetProperty("title").GetString()));
        Assert.False(string.IsNullOrWhiteSpace(problem.GetProperty("type").GetString()));
        return problem;
    }
}
