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
        public string? OrgUnitLogo { get; set; }
        public string? OrgUnitBanner { get; set; }
        public string? OrgUnitMotto { get; set; }
        public string? OrgUnitWebsite { get; set; }
        public string? OrgUnitEmail { get; set; }
        public string? OrgUnitPhone { get; set; }
        public string? OrgUnitAddress { get; set; }
        public string? OrgUnitColorPrimary { get; set; }
        public string? OrgUnitColorSecondary { get; set; }
        public string? OrgUnitColorTertiary { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? ArchivedOn { get; set; }
    }
}
