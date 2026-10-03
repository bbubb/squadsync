using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SquadSync.Application.Rosters;

namespace SquadSync.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSquadSyncPersistence(
        this IServiceCollection services,
        string? connectionString)
    {
        services.AddDbContext<SquadSyncDbContext>(options =>
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                options.UseNpgsql(connectionString);
            }
        });

        services.AddScoped<IRosterPersistence, EfRosterPersistence>();
        return services;
    }
}
