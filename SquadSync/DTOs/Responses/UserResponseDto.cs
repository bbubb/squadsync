

namespace SquadSync.DTOs.Responses
{
    public class UserResponseDto
    {
        public Guid Guid { get; set; }
        public Guid RoleBearerGuid { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string UserStatus { get; set; }
        public ICollection<RoleDto>? RoleDtos { get; set; }
        public ICollection<RoleRequestDto>? RoleRequestDtos { get; set; }
        public ICollection<OrgUnitDto>? OwnedOrgUnitDtos { get; set; }
    }
}