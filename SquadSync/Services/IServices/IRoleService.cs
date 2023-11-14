using SquadSync.DTOs;
using SquadSync.Utilities;
using SquadSync.DTOs.Requests;

namespace SquadSync.Services.IServices
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
        Task<ServiceResult<RoleDto>> AddRoleToUserAsync(Guid userGuid, RoleCreateRequestDto dto);

    }
}
