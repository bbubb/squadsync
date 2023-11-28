using SquadSync.Data.Models;
using SquadSync.DTOs.Responses;

namespace SquadSync.DTOs.Requests
{
    public class OrgUnitUpdateRequestDto
    {
        public Guid Guid { get; set; }
        public string? OrgUnitName { get; set; }
        public string? OrgUnitDescription { get; set; }
        public string? OrgUnitStatus { get; set; }
        public UserResponseDto? Owner { get; set; }
        public ICollection<IRoleBearer>? OUMembers { get; set; }
        public ICollection<Role>? OURoles { get; set; }
        public ICollection<RoleRequestDto>? OURoleRequests { get; set; }
        public ICollection<Role>? Roles { get; set; }
        public ICollection<RoleRequestDto>? RoleRequests { get; set; }
        public DateTime? Modified { get; set; }
    }
}
