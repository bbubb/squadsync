using SquadSync.Data.Models;
using SquadSync.DTOs;
using SquadSync.DTOs.Requests;
using SquadSync.Enums;

namespace SquadSync.Data.Repositories.IRepositories
{
    public interface IRoleRepository
    {
        Task<IEnumerable<User>> GetUsersByRoleAsync(RoleNameEnum role);
        Task<IEnumerable<Role>> GetRolesByUserGuidAsync (Guid guid);
        Task AssignRoleToUserAsync(Guid guid, RoleCreateRequestDto roleCreateRequestDto);
        Task RemoveRoleFromUserAsync(Guid guid, Guid roleGuid);
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<IEnumerable<User>> GetUserWithMultipleRolesAsync(RoleNameEnum[] roles);
    }
}
