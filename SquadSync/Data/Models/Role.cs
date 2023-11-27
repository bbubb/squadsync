using SquadSync.Enums;

namespace SquadSync.Data.Models
{
    public class Role
    {
        public Guid Guid { get; set; }
        public long RoleId { get; set; }
        public string RoleName { get; set; }
        public string? RoleDescription { get; set; }
        public long RoleBearerId { get; set; }
        public virtual IRoleBearer RoleBearer { get; set; }
        public long OrgUnitId { get; set; }
        public virtual OrgUnit OrgUnit { get; set; }
        public long RoleRequestId { get; set; }
        public virtual RoleRequest RoleRequest { get; set; }
        public ICollection<Permission> Permissions { get; set; }
        public RoleStatusEnum RoleStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public DateTime? ArchivedOn { get; set; }
    }
}
