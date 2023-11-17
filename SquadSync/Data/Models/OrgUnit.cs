using SquadSync.Enums;

namespace SquadSync.Data.Models
{
    public interface OrgUnit
    {
        public Guid Guid { get; set; }
        public long OrgUnitId { get; set; }
        public string OrgUnitName { get; set; }
        public string? OrgUnitDescription { get; set; }
        public IList<User> Users { get; set; }
        public IList<Role> Roles { get; set; }
        public IList<RoleRequest>? RoleRequests { get; set; }
        public OrgUnitStatusEnum OrgUnitStatus { get; set; }
        public User Owner { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? ArchivedOn { get; set; }
    }
}
