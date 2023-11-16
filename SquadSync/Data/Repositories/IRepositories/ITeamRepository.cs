using SquadSync.Data.Models;

namespace SquadSync.Data.Repositories.IRepositories
{
    public interface ITeamRepository
    {
        Task ArchiveTeamAsync(Team team);
        Task CreateTeamAsync(Team team);
        Task DeleteTeamAsync(Team team);
        Task<IEnumerable<Team>> GetAllActiveTeamsAsync();
        Task<IEnumerable<Team>> GetAllTeamsAsync();
        Task<Team> GetTeamByGuidAsync(Guid guid);
        Task<IEnumerable<Team>> GetTeamsByNameAsync(string name);
        Task UpdateTeamAsync(Team team);
    }
}
