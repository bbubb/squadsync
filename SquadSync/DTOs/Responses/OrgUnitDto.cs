namespace SquadSync.DTOs.Responses
{
    public class OrgUnitDto
    {
        public Guid Guid { get; set; }
        public string OrgUnitName { get; set; }
        public string? OrgUnitDescription { get; set; }
        public string OrgUnitStatus { get; set; }
        public UserResponseDto Owner { get; set; }
        public IList<UserResponseDto> Users { get; set; }
        public IList<RoleDto> Roles { get; set; }
        public IList<RoleRequestDto>? RoleRequests { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? ArchivedOn { get; set; }
    }
}
