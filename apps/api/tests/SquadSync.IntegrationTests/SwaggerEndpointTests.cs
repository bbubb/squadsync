using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace SquadSync.IntegrationTests;

public sealed class SwaggerEndpointTests
{
    [Fact]
    public async Task DevelopmentSwagger_DescribesPeopleAndTeamsMethodsAndResponses()
    {
        await using var factory = new ApiTestFactory();
        using var client = factory.CreateClient();
        using var response = await client.GetAsync("/swagger/v1/swagger.json");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var document = await response.Content.ReadFromJsonAsync<JsonElement>();
        var paths = document.GetProperty("paths");
        Assert.Equal(new[] { "/api/people", "/api/people/{id}", "/api/teams", "/api/teams/{id}" },
            paths.EnumerateObject().Select(path => path.Name).Order().ToArray());
        foreach (var route in new[] { "/api/people", "/api/teams" })
        {
            var create = paths.GetProperty(route);
            Assert.Equal("post", Assert.Single(create.EnumerateObject()).Name);
            Assert.Equal(new[] { "201", "400" }, create.GetProperty("post").GetProperty("responses")
                .EnumerateObject().Select(status => status.Name).Order().ToArray());
            var read = paths.GetProperty(route + "/{id}");
            Assert.Equal("get", Assert.Single(read.EnumerateObject()).Name);
            Assert.Equal(new[] { "200", "400", "404" }, read.GetProperty("get").GetProperty("responses")
                .EnumerateObject().Select(status => status.Name).Order().ToArray());
        }
    }
}
