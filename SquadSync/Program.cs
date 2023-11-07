using Microsoft.EntityFrameworkCore;
using Asp.Versioning;
using Serilog;
using SquadSync.Data;
using SquadSync.MappingProfiles;
using SquadSync.Utilities;
using SquadSync.Utilities.IUtilities;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Debug()
    .WriteTo.Seq("http://localhost:5341") // URL of Seq server
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddDbContext<SQLDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("MySqlConnection"),
        new MySqlServerVersion(new Version(8, 0, 34))));
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
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true; // adds the API versions in the response header
    options.ApiVersionReader = new UrlSegmentApiVersionReader(); // uses URL segment for versioning
});
   
// Registering the services
builder.Services.AddTransient<IEmailNormalizationUtilityService, EmailNormalizationUtilityService>();
builder.Services.AddTransient<IEmailValidationUtilityService, EmailValidationUtilityService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
