using SquadSync.Enums;

namespace SquadSync.Data.Models
{
    public class Team
    {
        public Guid Guid { get; set; }
        public long TeamId { get; set; }
        public string TeamName { get; set; }
        public string TeamDescription { get; set; }
        public IList<User> Users { get; set; }
        public IList<Role> Roles { get; set; }
        public IList<RoleRequest> RoleRequests { get; set; }
        public TeamStatusEnum TeamStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? ArchivedOn { get; set; }
    }
}
