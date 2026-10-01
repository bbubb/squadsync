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
var connectionString = builder.Configuration.GetConnectionString("SquadSync");
builder.Services.AddSquadSyncPersistence(connectionString);
builder.Services.AddHealthChecks()
    .AddCheck("postgres", new PostgresHealthCheck(connectionString));

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => false
});
app.MapHealthChecks("/health/ready");
app.MapControllers();

app.Run();

public partial class Program
{
}
