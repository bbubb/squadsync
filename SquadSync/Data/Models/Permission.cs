namespace SquadSync.Data.Models
{
    public class Permission
    {
        public Guid Guid { get; set; }
        public long PermissionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual Role Role { get; set; }
        public long RoleId { get; set; }
        public bool IsActive { get; set; }
    }
}
