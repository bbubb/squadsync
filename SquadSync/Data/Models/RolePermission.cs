using SquadSync.Data.Models;

namespace SquadSync.Data.Models
{
    public class RolePermission
    {
        public long RolePermissionId { get; set; }
        public long RoleId { get; set; }
        public virtual Role Role { get; set; }
        public long PermissionId { get; set; }
        public virtual Permission Permission { get; set; }
    }
}
