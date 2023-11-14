using Microsoft.EntityFrameworkCore;
using SquadSync.Data.Models;

namespace SquadSync.Data
{
    public class SQLDbContext : DbContext
    {
        public SQLDbContext(DbContextOptions<SQLDbContext> options) : base(options)
        { }

        //DbSets for tables
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleRequest> RoleRequests { get; set; }
        public DbSet<Team> Teams { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureUserEntity(modelBuilder);
            ConfigureRoleEntity(modelBuilder);
            ConfigureRoleRequestEntity(modelBuilder);
            ConfigureTeamEntity(modelBuilder);
        }

        // User
        private void ConfigureUserEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);
                entity.Property(u => u.UserName).IsRequired();
                entity.Property(u => u.Email).IsRequired();

                // User to Role is Many-to-One
                entity.HasMany(u => u.Roles)
                    .WithOne(r => r.User);

                // User to RoleRequest is Many-to-One
                entity.HasMany(u => u.RoleRequests)
                    .WithOne(rr => rr.User);
            });
        }

        //Role
        private void ConfigureRoleEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.RoleId);

                // Role to Team is One-to-Many
                entity.HasOne(r => r.Team)
                      .WithMany(t => t.Roles)
                      .HasForeignKey(r => r.TeamId)
                      .IsRequired();

                // Role to User is One-to-Many
                entity.HasOne(r => r.User)
                      .WithMany(u => u.Roles)
                      .HasForeignKey(r => r.UserId)
                      .IsRequired();

                // Role to RoleRequest is One-to-One
                entity.HasOne(r => r.RoleRequest)
                      .WithOne(rr => rr.Role)
                      .HasForeignKey<RoleRequest>(rr => rr.RoleId)
                      .IsRequired();
            });
        }

        private void ConfigureRoleRequestEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoleRequest>(entity =>
            {
                entity.HasKey(rr => rr.RoleRequestId);

                // One-to-One relationship between Role and RoleRequest
                entity.HasOne(rr => rr.Role)
                      .WithOne(r => r.RoleRequest)
                      .HasPrincipalKey<RoleRequest>(rr => rr.RoleId)
                      .IsRequired();

                // One-to-Many relationship between User and RoleRequest
                entity.HasOne(rr => rr.User)
                      .WithMany (u => u.RoleRequests)
                      .HasForeignKey(rr => rr.UserId)
                      .IsRequired();

                // One-to-Many relationship between Team and RoleRequest
                entity.HasOne(rr => rr.Team)
                      .WithMany(t => t.RoleRequests)
                      .HasForeignKey(rr => rr.TeamId)
                      .IsRequired();
            });
        }

        private void ConfigureTeamEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(t => t.TeamId);
                entity.Property(t => t.TeamName).IsRequired();

                // Team to Role is Many-to-One
                entity.HasMany(t => t.Roles)
                      .WithOne(r => r.Team)
                      .HasForeignKey(r => r.TeamId)
                      .IsRequired();

                // Team to RoleRequest is Many-to-One
                entity.HasMany(t => t.RoleRequests)
                      .WithOne(rr => rr.Team)
                      .HasForeignKey(rr => rr.TeamId);
            });
        }
    }
}
