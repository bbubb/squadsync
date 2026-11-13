using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace SquadSync.IntegrationTests;

public sealed class SwaggerEndpointTests
{
    [Theory]
    [InlineData("MembershipResponse", new[] { "id", "person", "teamId", "teamRole" }, "MembershipPersonResponse")]
    [InlineData("MembershipPersonResponse", new[] { "firstName", "id", "lastName" }, null)]
    [InlineData("RosterEntryResponse", new[] { "id", "jerseyNumber", "person", "rosterStatus", "teamMembershipId" }, "RosterPersonResponse")]
    [InlineData("RosterPersonResponse", new[] { "firstName", "id", "lastName" }, null)]
    public async Task DevelopmentSwagger_MembershipAndRosterResponsesDescribeRequiredProperties(
        string schemaName, string[] expectedProperties, string? personSchemaName)
    {
        await using var factory = new ApiTestFactory();
        using var client = factory.CreateClient();
        using var response = await client.GetAsync("/swagger/v1/swagger.json");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var document = await response.Content.ReadFromJsonAsync<JsonElement>();
        var schema = document.GetProperty("components").GetProperty("schemas").GetProperty(schemaName);

        Assert.Equal(expectedProperties, schema.GetProperty("required")
            .EnumerateArray().Select(property => property.GetString()).Order().ToArray());
        var properties = schema.GetProperty("properties");
        Assert.Equal(expectedProperties, properties.EnumerateObject().Select(property => property.Name).Order().ToArray());
        foreach (var propertyName in expectedProperties)
        {
            var property = properties.GetProperty(propertyName);
            var isNullable = property.TryGetProperty("nullable", out var nullable) && nullable.GetBoolean();
            Assert.Equal(propertyName == "jerseyNumber", isNullable);
            if (propertyName == "person")
            {
                Assert.Equal($"#/components/schemas/{personSchemaName}", property.GetProperty("$ref").GetString());
            }
            else
            {
                Assert.Equal("string", property.GetProperty("type").GetString());
                if (propertyName is "id" or "teamId" or "teamMembershipId")
                    Assert.Equal("uuid", property.GetProperty("format").GetString());
            }
        }
    }

    [Fact]
    public async Task DevelopmentSwagger_TeamResponseHasRequiredNonNullableProperties()
    {
        await using var factory = new ApiTestFactory();
        using var client = factory.CreateClient();
        using var response = await client.GetAsync("/swagger/v1/swagger.json");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var document = await response.Content.ReadFromJsonAsync<JsonElement>();
        var team = document.GetProperty("components").GetProperty("schemas").GetProperty("TeamResponse");

        Assert.Equal(new[] { "id", "name" }, team.GetProperty("required")
            .EnumerateArray().Select(property => property.GetString()).Order().ToArray());
        var properties = team.GetProperty("properties");
        Assert.Equal(new[] { "id", "name" }, properties.EnumerateObject().Select(property => property.Name).Order().ToArray());
        foreach (var propertyName in new[] { "id", "name" })
        {
            var property = properties.GetProperty(propertyName);
            Assert.Equal("string", property.GetProperty("type").GetString());
            Assert.False(property.TryGetProperty("nullable", out var nullable) && nullable.GetBoolean());
        }
        Assert.Equal("uuid", properties.GetProperty("id").GetProperty("format").GetString());
    }

    [Fact]
    public async Task DevelopmentSwagger_DescribesPeopleAndTeamsMethodsAndResponses()
    {
        await using var factory = new ApiTestFactory();
        using var client = factory.CreateClient();
        using var response = await client.GetAsync("/swagger/v1/swagger.json");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var document = await response.Content.ReadFromJsonAsync<JsonElement>();
        var paths = document.GetProperty("paths");
        Assert.Equal(new[] { "/api/people", "/api/people/{id}", "/api/people/{personId}/player-profile", "/api/people/candidates", "/api/teams", "/api/teams/{id}",
                "/api/teams/{teamId}/memberships", "/api/teams/{teamId}/memberships/{membershipId}",
                "/api/teams/{teamId}/roster", "/api/teams/{teamId}/roster/{rosterEntryId}", "/api/teams/{teamId}/roster/players" },
            paths.EnumerateObject().Select(path => path.Name).Order().ToArray());
        var profile = paths.GetProperty("/api/people/{personId}/player-profile");
        Assert.Equal(new[] { "get", "post", "put" }, profile.EnumerateObject().Select(method => method.Name).Order().ToArray());
        Assert.Equal(new[] { "201", "400", "404", "409" }, profile.GetProperty("post").GetProperty("responses")
            .EnumerateObject().Select(status => status.Name).Order().ToArray());
        Assert.Equal(new[] { "200", "400", "404" }, profile.GetProperty("get").GetProperty("responses")
            .EnumerateObject().Select(status => status.Name).Order().ToArray());
        Assert.Equal(new[] { "200", "400", "404" }, profile.GetProperty("put").GetProperty("responses")
            .EnumerateObject().Select(status => status.Name).Order().ToArray());
        foreach (var (method, status) in new[] { ("post", "201"), ("get", "200"), ("put", "200") })
        {
            Assert.Equal("#/components/schemas/PlayerProfileResponse", profile.GetProperty(method).GetProperty("responses")
                .GetProperty(status).GetProperty("content").GetProperty("application/json").GetProperty("schema").GetProperty("$ref").GetString());
        }
        foreach (var route in new[] { "/api/people", "/api/teams" })
        {
            var create = paths.GetProperty(route);
            Assert.Equal(route == "/api/teams" ? new[] { "get", "post" } : new[] { "post" },
                create.EnumerateObject().Select(method => method.Name).Order().ToArray());
            Assert.Equal(new[] { "201", "400" }, create.GetProperty("post").GetProperty("responses")
                .EnumerateObject().Select(status => status.Name).Order().ToArray());
            var read = paths.GetProperty(route + "/{id}");
            Assert.Equal("get", Assert.Single(read.EnumerateObject()).Name);
            Assert.Equal(new[] { "200", "400", "404" }, read.GetProperty("get").GetProperty("responses")
                .EnumerateObject().Select(status => status.Name).Order().ToArray());
        }
        var teams = paths.GetProperty("/api/teams").GetProperty("get").GetProperty("responses");
        Assert.Equal("200", Assert.Single(teams.EnumerateObject()).Name);
        var teamsSchema = teams.GetProperty("200").GetProperty("content").GetProperty("application/json").GetProperty("schema");
        Assert.Equal("array", teamsSchema.GetProperty("type").GetString());
        Assert.Equal("#/components/schemas/TeamResponse", teamsSchema.GetProperty("items").GetProperty("$ref").GetString());
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
