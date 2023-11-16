using SquadSync.Data.Models;

namespace SquadSync.Data.Repositories.IRepositories
{
    public interface ITeamRepository
    {
        Task<IEnumerable<Team>> GetAllTeamsAsync();
        Task<IEnumerable<Team>> GetAllActiveTeamsAsync();
        Task<Team> GetTeamByGuidAsync(Guid guid);
        Task<IEnumerable<Team>> GetTeamsByNameAsync(string name);
        Task CreateTeamAsync(Team team);
        Task UpdateTeamAsync(Team team);
        Task ArchiveTeamAsync(Team team);
        Task DeleteTeamAsync(Team team);
    }
}
