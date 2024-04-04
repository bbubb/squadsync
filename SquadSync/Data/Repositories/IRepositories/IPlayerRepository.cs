using SquadSync.Data.Models;

namespace SquadSync.Data.Repositories.IRepositories
{
    public interface IPlayerRepository
    {
        Task ArchivePlayerAsync(Player player);
        Task CreatePlayerAsync(Player player);
        Task DeletePlayerAsync(Player player);
        Task<IEnumerable<Player>> GetAllPlayersAsync();
        Task<Player> GetPlayerByGuidAsync(Guid guid);
        Task<IEnumerable<Player>> GetPlayersByTeamAsync(Team team);
        Task UpdatePlayerAsync(Player player);
    }
}
