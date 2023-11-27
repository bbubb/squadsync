using System.Collections.ObjectModel;
using SquadSync.Data.Models;
using SquadSync.DTOs.Responses;
using SquadSync.Enums;

namespace SquadSync.DTOs
{
    public class RoleDto
    {
        public Guid Guid { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public RoleStatusEnum RoleStatus { get; set; }
        public IRoleBearer RoleBearer { get; set; }
        public OrgUnitDto OrgUnitDto { get; set; }
        public RoleRequestDto RoleRequestDto { get; set; }
        public Collection<PermissionDto> PermissionDtos { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
