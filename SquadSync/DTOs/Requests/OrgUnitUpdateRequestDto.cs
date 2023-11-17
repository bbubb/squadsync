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
        public IList<UserResponseDto>? Users { get; set; }
        public IList<RoleDto>? Roles { get; set; }
        public IList<RoleRequestDto>? RoleRequests { get; set; }
        public DateTime? Modified { get; set; }
    }
}
