using Microsoft.EntityFrameworkCore;
using Asp.Versioning;
using Serilog;
using SquadSync.Data;
using SquadSync.MappingProfiles;
using SquadSync.Utilities;
using SquadSync.Utilities.IUtilities;
using SquadSync.Middleware;
using System.Diagnostics;
using SquadSync.Services.IServices;
using SquadSync.Services;
using SquadSync.Data.Repositories.IRepositories;
using SquadSync.Data.Repositories;
using Pomelo.EntityFrameworkCore.MySql.Internal;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Debug()
    .WriteTo.Seq("http://localhost:5318") // URL of Seq server
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddDbContext<SQLDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("SQLDbConnection"),
        new MySqlServerVersion(new Version(8, 0, 34)),
        MySqlOptions => MySqlOptions.EnableRetryOnFailure()));
builder.Services.AddAuthorization();
builder.Services.AddAutoMapper(typeof(UserMappingProfile).Assembly);
builder.Services.AddAuthentication();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add MVC services
builder.Services.AddControllers();
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0); // default version 1.0
    //options.AssumeDefaultVersionWhenUnspecified = true; // disabling: recommended to enable for legacy only
    options.ReportApiVersions = true; // adds the API versions in the response header
    options.ApiVersionReader = new UrlSegmentApiVersionReader(); // uses URL segment for versioning
})
    .AddMvc();
   
// Registering the services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddTransient<IEmailNormalizationService, EmailNormalizationService>();
builder.Services.AddTransient<IEmailValidationService, EmailValidationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

Log.Information("Program: Starting up the application");
app.Run();