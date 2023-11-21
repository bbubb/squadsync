using Microsoft.EntityFrameworkCore;
using SquadSync.Data.Models;
using SquadSync.Data.Repositories.IRepositories;
using SquadSync.Exceptions;

namespace SquadSync.Data.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        SQLDbContext _dbContext;
        ILogger<PermissionRepository> _logger;
        public PermissionRepository(
            SQLDbContext dbContext,
            ILogger<PermissionRepository> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ArchivePermissionAsync(Permission permission)
        {
            _logger.LogDebug("PermissionRepository: Start archiving permission with GUID: {Guid}", permission.Guid);

            if (permission == null)
            {
                _logger.LogWarning("PermissionRepository: Permission is null");
                throw new CustomArgumentNullException("PermissionRepository, ", nameof(permission));
            }

            _dbContext.Permissions.Update(permission);
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug("PermissionRepository: Finished archiving permission with GUID: {Guid}", permission.Guid);
        }

        public async Task CreatePermissionAsync(Permission permission)
        {
            _logger.LogDebug("PermissionRepository: Start creating permission with GUID: {Guid}", permission.Guid);

            if (permission == null)
            {
                _logger.LogWarning("PermissionRepository: Permission is null");
                throw new CustomArgumentNullException("PermissionRepository, ", nameof(permission));
            }

            _logger.LogDebug("PermissionRepository: Creating permission with GUID: {Guid}", permission.Guid);
            _dbContext.Permissions.Add(permission);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeletePermissionAsync(Permission permission)
        {
            _logger.LogDebug("PermissionRepository: Start deleting permission with GUID: {Guid}", permission.Guid);

            if (permission == null)
            {
                _logger.LogWarning("PermissionRepository: Permission is null");
                throw new CustomArgumentNullException("PermissionRepository, ", nameof(permission));
            }

            _dbContext.Permissions.Remove(permission);
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug("PermissionRepository: Finished deleting permission with GUID: {Guid}", permission.Guid);
        }

        public async Task<Permission> GetPermissionByGuidAsync(Guid permissionGuid)
        {
            _logger.LogDebug("PermissionRepository: Start getting permission with GUID: {Guid}", permissionGuid);

            var permission = await _dbContext.Permissions
                .FirstOrDefaultAsync(p => p.Guid == permissionGuid);

            if (permission == null)
            {
                _logger.LogWarning("PermissionRepository: Permission is null");
                throw new CustomArgumentNullException("PermissionRepository, ", nameof(permission));
            }
            return permission;
        }

        public async Task<IEnumerable<Permission>> GetPermissionsAsync()
        {
            _logger.LogDebug("PermissionRepostiory: Retrieving all permissions");

            return await _dbContext.Permissions.ToListAsync();
        }

        public async Task<IEnumerable<Permission>> GetPermissionsByRoleGuidAsync(Guid roleGuid)
        {
            _logger.LogDebug("PermissionRepository: Start retrieving permissions by role GUID: {Guid}", roleGuid);

            var permissions = await _dbContext.Permissions
                .Where(p => p.Role.Guid == roleGuid)
                .ToListAsync();

            if (permissions == null)
            {
                _logger.LogWarning("PermissionRepository: Permissions are null");
                throw new CustomArgumentNullException("PermissionRepository, ", nameof(permissions));
            }
            
            _logger.LogDebug("PermissionRepository: Finished retrieving permissions by role GUID: {Guid}", roleGuid);
            return permissions;
        }

        public async Task UpdatePermissionAsync(Permission permission)
        {
            _logger.LogDebug("PermissionRepository: Start updating permission with GUID: {Guid}", permission.Guid);

            if (permission == null)
            {
                _logger.LogWarning("PermissionRepository: Permission is null");
                throw new CustomArgumentNullException("PermissionRepository, ", nameof(permission));
            }

            _dbContext.Permissions.Update(permission);
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug("PermissionRepository: Finished updating permission with GUID: {Guid}", permission.Guid);
        }
    }
}
