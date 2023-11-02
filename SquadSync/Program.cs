using Microsoft.EntityFrameworkCore;
using SquadSync.Data;
using SquadSync.MappingProfiles;
using SquadSync.Utilities;
using SquadSync.Utilities.IUtilities;

var builder = WebApplication.CreateBuilder(args);

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
