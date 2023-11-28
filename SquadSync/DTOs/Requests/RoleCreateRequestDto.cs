using SquadSync.Data.Models;

namespace SquadSync.DTOs.Requests
{
    public class RoleCreateRequestDto
    {
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public Guid RoleBearerGuid { get; set; }
        public Guid OrgUnitGuid { get; set; }
        public string RoleStatus { get; set; }
        public ICollection<Guid> PermissionGuids { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
