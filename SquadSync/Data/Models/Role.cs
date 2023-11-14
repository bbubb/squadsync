using SquadSync.Enums;

namespace SquadSync.Data.Models
{
    public class Role
    {
        public Guid Guid { get; set; }
        public long RoleId { get; set; }
        public RoleNameEnum RoleName { get; set; }
        public long UserId { get; set; }
        public virtual User User { get; set; }
        public long TeamId { get; set; }
        public virtual Team Team { get; set; }
        public long RoleRequestId { get; set; }
        public virtual RoleRequest RoleRequest { get; set; }
        public RoleStatusEnum RoleStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public DateTime? ArchivedOn { get; set; }
    }
}
