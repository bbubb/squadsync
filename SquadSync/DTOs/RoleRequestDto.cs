using System.Collections.ObjectModel;
using SquadSync.Data.Models;
using SquadSync.Enums;

namespace SquadSync.DTOs
{
    public class RoleRequestDto
    {
        public Guid Guid { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public RoleStatusEnum RoleStatus { get; set; }
        public IRoleBearer Owner { get; set; }
        public Collection<IRoleBearer> RoleMembers { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
