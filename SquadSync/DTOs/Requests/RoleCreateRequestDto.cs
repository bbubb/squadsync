using SquadSync.Data.Models;

namespace SquadSync.DTOs.Requests
{
    public class RoleCreateRequestDto
    {
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public IRoleBearer Owner { get; set; }
        public Guid TeamGuid { get; set; }
        public string RoleStatus { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
