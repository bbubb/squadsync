using Microsoft.EntityFrameworkCore;
using SquadSync.Data.Models;

namespace SquadSync.Data
{
    public class SQLDbContext : DbContext
    {
        public SQLDbContext(DbContextOptions<SQLDbContext> options) : base(options)
        { }

        //DbSets for tables
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<OrgUnit> OrgUnits { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<RoleRequest> RoleRequests { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureUserEntity(modelBuilder);
            ConfigureRoleEntity(modelBuilder);
            ConfigureRoleRequestEntity(modelBuilder);
            ConfigureOrgUnitEntity(modelBuilder);
            ConfigurePermissionEntity(modelBuilder);
        }

        // User
        private void ConfigureUserEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);
                entity.Property(u => u.UserName).IsRequired();
                entity.Property(u => u.Email).IsRequired();
                entity.Property(u => u.RoleBearerGuid).IsRequired();
                entity.Property(u => u.RoleBearerId).IsRequired();

                // User to OrgUnit is Many-to-One
                entity.HasMany(u => u.OwnedOrgUnits)
                      .WithOne(ou => ou.Owner)
                      .HasForeignKey(ou => ou.OwnerId);
            });
        }

        // Feature
        private void ConfigureFeatureEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Feature>(entity =>
            {
                entity.HasKey(f => f.FeatureId);
                entity.Property(f => f.FeatureName).IsRequired();
                entity.Property(f => f.IsActiveFeature).IsRequired();
                entity.Property(f => f.CreatedOn).IsRequired();
                entity.Property(f => f.ArchivedOn).IsRequired();
            });
        }

        // Role
        private void ConfigureRoleEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.RoleId);

                // Role to RoleBearer is One-to-Many
                entity.HasOne(r => r.RoleBearer)
                      .WithMany(rb => rb.Roles)
                      .HasForeignKey(r => r.RoleBearerId)
                      .IsRequired();

                // Role to RoleRequest is One-to-One
                entity.HasOne(r => r.RoleRequest)
                      .WithOne(rr => rr.Role)
                      .HasForeignKey<RoleRequest>(rr => rr.RoleId)
                      .IsRequired();

                // Role to Permission is Many-to-One
                entity.HasMany(r => r.Permissions)
                      .WithOne(p => p.Role)
                      .HasForeignKey(p => p.RoleId)
                      .IsRequired();
            });
        }

        //RoleRequest
        private void ConfigureRoleRequestEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoleRequest>(entity =>
            {
                entity.HasKey(rr => rr.RoleRequestId);

                // RoleRequest to Role is One-to-One
                entity.HasOne(rr => rr.Role)
                      .WithOne(r => r.RoleRequest)
                      .HasPrincipalKey<RoleRequest>(rr => rr.RoleId)
                      .IsRequired();

                // RoleRequest to RoleBearer is One-to-Many
                entity.HasOne(rr => rr.RoleBearer)
                      .WithMany (rb => rb.RoleRequests)
                      .HasForeignKey(rr => rr.RoleBearerId)
                      .IsRequired();
            });
        }

        //OrgUnit
        private void ConfigureOrgUnitEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrgUnit>(entity =>
            {
                entity.HasKey(ou => ou.OrgUnitId);
                entity.Property(ou => ou.OrgUnitName).IsRequired();
                entity.Property(ou => ou.OrgUnitStatus).IsRequired();
                
                // OrgUnit to user Owner is One-to-Many
                entity.HasOne(ou => ou.Owner)
                      .WithMany(u => u.OwnedOrgUnits)
                      .HasForeignKey(ou => ou.OwnerId)
                      .IsRequired();
            });
        }

        //Permission
        private void ConfigurePermissionEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(p => p.PermissionId);
                entity.Property(p => p.PermissionName).IsRequired();
                entity.Property(p => p.IsActive).IsRequired();
                
                // Permission to Role is One-to-Many
                entity.HasOne(p => p.Role)
                      .WithMany(r => r.Permissions)
                      .HasForeignKey(p => p.RoleId)
                      .IsRequired();
            });
        }
    }
}
