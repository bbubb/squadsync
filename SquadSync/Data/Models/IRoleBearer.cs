namespace SquadSync.Data.Models
{
    public interface IRoleBearer
    {
        public Guid RoleBearerGuid { get; set; }
        public long RoleBearerId { get; set; }
        public ICollection<Role> Roles { get; set; }
        public ICollection<RoleRequest> RoleRequests { get; set; }
    }
}
