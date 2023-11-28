using SquadSync.Enums;

namespace SquadSync.Data.Models
{
    public class OrgUnit : IRoleBearer
    {
        public Guid Guid { get; set; }
        public long OrgUnitId { get; set; }
        public string OrgUnitName { get; set; }
        public string? OrgUnitDescription { get; set; }
        public OrgUnitStatusEnum OrgUnitStatus { get; set; }
        public ICollection<IRoleBearer> OUMembers { get; set; }
        public ICollection<Role> OURoles { get; set; }
        public ICollection<RoleRequest>? OURoleRequests { get; set; }
        // Implement IRoleBearer
        public Guid RoleBearerGuid 
        {
            get { return Guid; }
            set { Guid = value; }
        }
        public long RoleBearerId
        {
            get { return OrgUnitId; }
            set { OrgUnitId = value; }
        }
        public ICollection<Role>? Roles { get; set; }
        public ICollection<RoleRequest>? RoleRequests { get; set; }
        public User Owner { get; set; }
        public long OwnerId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? ArchivedOn { get; set; }
    }
}
