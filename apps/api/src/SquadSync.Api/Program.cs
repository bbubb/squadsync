using Serilog;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using SquadSync.Infrastructure;
using SquadSync.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
var connectionString = builder.Configuration.GetConnectionString("SquadSync");
builder.Services.AddSquadSyncPersistence(connectionString);
builder.Services.AddHealthChecks()
    .AddCheck("postgres", new PostgresHealthCheck(connectionString));

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

if (app.Configuration.GetValue<bool>("seed-demo"))
{
    if (!app.Environment.IsDevelopment())
    {
        throw new InvalidOperationException("Demo seeding is allowed only in the Development environment.");
    }

    await using var scope = app.Services.CreateAsyncScope();
    await scope.ServiceProvider.GetRequiredService<DevelopmentDemoSeeder>().SeedAsync();
    app.Logger.LogInformation("Development demo seed completed. SquadSync Demo FC is ready for inspection.");
    await app.DisposeAsync();
    return;
}

app.UseSerilogRequestLogging();
// Use safe standard errors in Development too; never return the developer exception page.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // All MVC controllers are management endpoints until an authenticated boundary is approved.
    app.MapControllers();
}

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => false
});
app.MapHealthChecks("/health/ready");

app.Run();

public partial class Program
{
}
