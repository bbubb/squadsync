using SquadSync.Data.Models;

namespace SquadSync.Data.Repositories.IRepositories
{
    public interface IPermissionRepository
    {  
        Task ArchivePermissionAsync(Permission permission);
        Task CreatePermissionAsync(Permission permission);
        Task DeletePermissionAsync(Permission permission);
        Task<Permission> GetPermissionByGuidAsync(Guid permissionGuid);
        Task<IEnumerable<Permission>> GetPermissionsAsync();
        Task<IEnumerable<Permission>> GetPermissionsByRoleGuidAsync(Guid roleGuid);
        Task UpdatePermissionAsync(Permission permission);
    }
}
