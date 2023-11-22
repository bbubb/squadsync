namespace SquadSync.Data.Models
{
    public interface IRoleBearer
    {
        public ICollection<Role> Roles { get; set; }
    }
}
