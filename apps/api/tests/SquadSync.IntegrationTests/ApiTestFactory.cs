using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace SquadSync.IntegrationTests;

public sealed class ApiTestFactory(
    string environment = "Development",
    string connectionString = "",
    bool includeProbe = false) : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Host configuration is applied before Program reads configuration for service registration.
        builder.ConfigureHostConfiguration(configuration => configuration.AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                ["ConnectionStrings:SquadSync"] = connectionString,
                ["seed-demo"] = "false"
            }));
        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(environment);
        if (includeProbe)
        {
            builder.ConfigureServices(services => services.AddControllers()
                .AddApplicationPart(typeof(ManagementProbeController).Assembly));
        }
    }
}
