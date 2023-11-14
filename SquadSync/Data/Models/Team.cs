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
    }
}
