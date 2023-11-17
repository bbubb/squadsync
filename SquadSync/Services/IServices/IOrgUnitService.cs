using SquadSync.DTOs.Requests;
using SquadSync.DTOs.Responses;
using SquadSync.Utilities;

namespace SquadSync.Services.IServices
{
    public interface IOrgUnitService
    {
        Task<ServiceResult<OrgUnitDto>> GetOrgUnitByGuidAsync(Guid orgGuid);
        Task<ServiceResult<IEnumerable<OrgUnitDto>>> GetAllOrgUnitDtosAsync();
        //Task<ServiceResult<IEnumerable<OrgUnitDto>>> GetOrgUnitDtosByUserGuidAsync(Guid userGuid);
        Task<ServiceResult<OrgUnitDto>> ArchiveOrgUnitAsync(Guid orgGuid);
        Task<ServiceResult<OrgUnitDto>> CreateOrgUnitAsync(OrgUnitCreateRequestDto dto);
        Task<ServiceResult> DeleteOrgUnitAsync(Guid orgGuid);
        Task<ServiceResult<OrgUnitDto>> UpdateOrgUnitAsync(Guid requestUserGuid, OrgUnitUpdateRequestDto dto);
    }
}
