using SquadSync.Data.Models;

namespace SquadSync.DTOs.Responses
{
    public class OrgUnitDto
    {
        public Guid Guid { get; set; }
        public Guid RoleBearerGuid { get; set; }
        public string OrgUnitName { get; set; }
        public string? OrgUnitDescription { get; set; }
        public string OrgUnitStatus { get; set; }
        public UserResponseDto Owner { get; set; }
        public ICollection<RoleDto> OURoles { get; set; }
        public ICollection<RoleRequestDto>? OURoleRequests { get; set; }
        public ICollection<IRoleBearer> OUMembers { get; set; }
        public ICollection<RoleDto>? Roles { get; set; }
        public ICollection<RoleRequestDto>? RoleRequests { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? ArchivedOn { get; set; }
    }
}
