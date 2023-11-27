using SquadSync.Data.Models;
using SquadSync.DTOs;
using SquadSync.DTOs.Requests;
using SquadSync.Enums;

namespace SquadSync.Data.Repositories.IRepositories
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<IEnumerable<IRoleBearer>> GetBearersByOURoleAsync(Role ouRole);
        Task<IEnumerable<IRoleBearer>> GetBearersWithMultipleOURolesAsync(Role[] ouRoles);
        Task<IEnumerable<Role>> GetRolesByBearerGuidAsync (Guid roleBearerguid);
        Task<Role> GetRoleByGuidAsync(Guid guid);
        Task CreateRoleAsync(Role role);
        Task UpdateRoleAsync(Role role);
        Task ArchiveRoleAsync(Role role);
        Task DeleteRoleAsync(Role role);
    }
}
