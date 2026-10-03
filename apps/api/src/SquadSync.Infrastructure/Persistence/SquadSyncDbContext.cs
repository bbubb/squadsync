using Microsoft.EntityFrameworkCore;
using SquadSync.Domain;

namespace SquadSync.Infrastructure.Persistence;

public sealed class SquadSyncDbContext(DbContextOptions<SquadSyncDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<Team> Teams => Set<Team>();

    public DbSet<TeamMembership> TeamMemberships => Set<TeamMembership>();

    public DbSet<PlayerProfile> PlayerProfiles => Set<PlayerProfile>();

    public DbSet<RosterEntry> RosterEntries => Set<RosterEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SquadSyncDbContext).Assembly);
    }
}
