using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using SquadSync.Domain;
using SquadSync.Infrastructure.Persistence;
using Xunit;

namespace SquadSync.IntegrationTests;

public class HealthEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetHealth_ReturnsOk()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetHealth_RemainsOkWhenPostgresIsUnavailable()
    {
        await using var unavailableDatabaseFactory = factory.WithWebHostBuilder(builder =>
            builder.ConfigureAppConfiguration((_, configuration) => configuration.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["ConnectionStrings:SquadSync"] = "Host=127.0.0.1;Port=1;Database=squadsync;Username=test;Password=test;Timeout=1;"
                })));
        using var client = unavailableDatabaseFactory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetHealth_RemainsOkWhenPostgresIsNotConfigured()
    {
        await using var unconfiguredDatabaseFactory = factory.WithWebHostBuilder(builder =>
            builder.ConfigureAppConfiguration((_, configuration) => configuration.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["ConnectionStrings:SquadSync"] = string.Empty
                })));
        using var client = unconfiguredDatabaseFactory.CreateClient();

        var livenessResponse = await client.GetAsync("/health");
        var readinessResponse = await client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.OK, livenessResponse.StatusCode);
        Assert.Equal(HttpStatusCode.ServiceUnavailable, readinessResponse.StatusCode);
    }

    [Fact]
    public async Task GetReady_ReturnsServiceUnavailableWhenPostgresIsUnavailable()
    {
        await using var unavailableDatabaseFactory = factory.WithWebHostBuilder(builder =>
            builder.ConfigureAppConfiguration((_, configuration) => configuration.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["ConnectionStrings:SquadSync"] = "Host=127.0.0.1;Port=1;Database=squadsync;Username=test;Password=test;Timeout=1;"
                })));
        using var client = unavailableDatabaseFactory.CreateClient();

        var response = await client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.DoesNotContain("Password=test", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public void SquadSyncDbContext_UsesNpgsqlAndMapsCurrentEntities()
    {
        var services = new ServiceCollection()
            .AddSquadSyncPersistence("Host=localhost;Database=squadsync;Username=test;Password=test")
            .BuildServiceProvider();
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SquadSyncDbContext>();
        var model = dbContext.Model;

        Assert.Equal("Npgsql.EntityFrameworkCore.PostgreSQL", dbContext.Database.ProviderName);
        Assert.Equal(
            [nameof(Team), nameof(TeamMembership), nameof(User)],
            model.GetEntityTypes().Select(entityType => entityType.ClrType.Name).OrderBy(name => name));

        var user = model.FindEntityType(typeof(User))!;
        Assert.Equal(
            ["FirstName", "Id", "LastName"],
            user.GetProperties().Select(property => property.Name).OrderBy(name => name));
        Assert.Equal("Id", user.FindPrimaryKey()!.Properties.Single().Name);
        Assert.Equal(ValueGenerated.Never, user.FindProperty(nameof(User.Id))!.ValueGenerated);
        Assert.All(user.GetProperties(), property => Assert.False(property.IsNullable));

        var team = model.FindEntityType(typeof(Team))!;
        Assert.Equal(["Id", "Name"], team.GetProperties().Select(property => property.Name).OrderBy(name => name));
        Assert.Equal("Id", team.FindPrimaryKey()!.Properties.Single().Name);
        Assert.Equal(ValueGenerated.Never, team.FindProperty(nameof(Team.Id))!.ValueGenerated);
        Assert.All(team.GetProperties(), property => Assert.False(property.IsNullable));

        var membership = model.FindEntityType(typeof(TeamMembership))!;
        Assert.Equal(
            ["Id", "TeamId", "TeamRole", "UserId"],
            membership.GetProperties().Select(property => property.Name).OrderBy(name => name));
        Assert.Equal("Id", membership.FindPrimaryKey()!.Properties.Single().Name);
        Assert.Equal(ValueGenerated.Never, membership.FindProperty(nameof(TeamMembership.Id))!.ValueGenerated);
        Assert.All(membership.GetProperties(), property => Assert.False(property.IsNullable));
        Assert.Empty(membership.GetNavigations());

        var foreignKeys = membership.GetForeignKeys().ToArray();
        Assert.Equal(2, foreignKeys.Length);
        Assert.All(foreignKeys, foreignKey =>
        {
            Assert.True(foreignKey.IsRequired);
            Assert.Equal(DeleteBehavior.Restrict, foreignKey.DeleteBehavior);
        });
        Assert.Contains(foreignKeys, foreignKey =>
            foreignKey.Properties.Single().Name == nameof(TeamMembership.UserId) &&
            foreignKey.PrincipalEntityType.ClrType == typeof(User));
        Assert.Contains(foreignKeys, foreignKey =>
            foreignKey.Properties.Single().Name == nameof(TeamMembership.TeamId) &&
            foreignKey.PrincipalEntityType.ClrType == typeof(Team));
        Assert.Contains(membership.GetIndexes(), index => index.IsUnique &&
            index.Properties.Select(property => property.Name)
                .SequenceEqual([nameof(TeamMembership.UserId), nameof(TeamMembership.TeamId)]));
        Assert.Equal(typeof(string), membership.FindProperty(nameof(TeamMembership.TeamRole))!
            .GetTypeMapping().Converter!.ProviderClrType);
    }
}
