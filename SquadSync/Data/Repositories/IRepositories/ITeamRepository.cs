using SquadSync.Data.Models;

namespace SquadSync.Data.Repositories.IRepositories
{
    public interface ITeamRepository
    {
        Task<IEnumerable<Team>> GetTeamsByPlayers(IEnumerable<Player> players);
    }
}
