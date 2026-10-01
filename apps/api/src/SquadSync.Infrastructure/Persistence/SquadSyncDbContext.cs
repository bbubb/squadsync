using Microsoft.EntityFrameworkCore;
using SquadSync.Domain;

namespace SquadSync.Infrastructure.Persistence;

public sealed class SquadSyncDbContext(DbContextOptions<SquadSyncDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<Team> Teams => Set<Team>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SquadSyncDbContext).Assembly);
    }
}
