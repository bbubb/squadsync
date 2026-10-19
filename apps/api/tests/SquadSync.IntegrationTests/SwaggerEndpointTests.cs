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
        Assert.Equal(new[] { "/api/people", "/api/people/{id}", "/api/teams", "/api/teams/{id}",
                "/api/teams/{teamId}/memberships", "/api/teams/{teamId}/memberships/{membershipId}",
                "/api/teams/{teamId}/roster", "/api/teams/{teamId}/roster/{rosterEntryId}" },
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
        var roster = paths.GetProperty("/api/teams/{teamId}/roster");
        Assert.Equal(new[] { "get", "post" }, roster.EnumerateObject().Select(method => method.Name).Order().ToArray());
        Assert.Equal(new[] { "200", "400", "404" }, roster.GetProperty("get").GetProperty("responses")
            .EnumerateObject().Select(status => status.Name).Order().ToArray());
        var rosterSchema = roster.GetProperty("get").GetProperty("responses").GetProperty("200").GetProperty("content")
            .GetProperty("application/json").GetProperty("schema");
        Assert.Equal("array", rosterSchema.GetProperty("type").GetString());
        Assert.Equal("#/components/schemas/RosterEntryResponse", rosterSchema.GetProperty("items").GetProperty("$ref").GetString());
        Assert.Equal(new[] { "201", "400", "404", "409" }, roster.GetProperty("post").GetProperty("responses")
            .EnumerateObject().Select(status => status.Name).Order().ToArray());
        var rosterRead = paths.GetProperty("/api/teams/{teamId}/roster/{rosterEntryId}");
        Assert.Equal("get", Assert.Single(rosterRead.EnumerateObject()).Name);
        Assert.Equal(new[] { "200", "400", "404" }, rosterRead.GetProperty("get").GetProperty("responses")
            .EnumerateObject().Select(status => status.Name).Order().ToArray());
        var membership = paths.GetProperty("/api/teams/{teamId}/memberships");
        Assert.Equal(new[] { "get", "post" }, membership.EnumerateObject().Select(method => method.Name).Order().ToArray());
        Assert.Equal(new[] { "201", "400", "404", "409" }, membership.GetProperty("post").GetProperty("responses")
            .EnumerateObject().Select(status => status.Name).Order().ToArray());
        Assert.Equal(new[] { "200", "400", "404" }, membership.GetProperty("get").GetProperty("responses")
            .EnumerateObject().Select(status => status.Name).Order().ToArray());
        var schema = membership.GetProperty("get").GetProperty("responses").GetProperty("200").GetProperty("content")
            .GetProperty("application/json").GetProperty("schema");
        Assert.Equal("array", schema.GetProperty("type").GetString());
        Assert.Equal("#/components/schemas/MembershipResponse", schema.GetProperty("items").GetProperty("$ref").GetString());
        var membershipRead = paths.GetProperty("/api/teams/{teamId}/memberships/{membershipId}");
        Assert.Equal("get", Assert.Single(membershipRead.EnumerateObject()).Name);
        Assert.Equal(new[] { "200", "400", "404" }, membershipRead.GetProperty("get").GetProperty("responses")
            .EnumerateObject().Select(status => status.Name).Order().ToArray());
    }
}
