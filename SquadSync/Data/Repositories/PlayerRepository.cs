using Microsoft.EntityFrameworkCore;
using SquadSync.Data.Models;
using SquadSync.Data.Repositories.IRepositories;
using SquadSync.Exceptions;

namespace SquadSync.Data.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly SQLDbContext _dbContext;
        private readonly ILogger<PlayerRepository> _logger;

        public PlayerRepository(
                       SQLDbContext dbContext,
                       ILogger<PlayerRepository> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ArchivePlayerAsync(Player player)
        {
            _logger.LogDebug("PlayerRepository: Start archiving player with GUID: {Guid}", player.PlayerGuid);

            if (player == null)
            {
                _logger.LogWarning("PlayerRepository: Player is null.");
                throw new CustomArgumentNullException("PlayerRepository", nameof(player.PlayerGuid));
            }

            _dbContext.Players.Update(player);
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug("PlayerRepository: Finished archiving player with GUID: {Guid}", player.PlayerGuid);
        }

        public async Task CreatePlayerAsync(Player player)
        {
            _logger.LogDebug("PlayerRepository: Start creating a new player with name: {PlayerName}", player.PlayerName);

            if (player == null) 
            {
                _logger.LogWarning("PlayerRepository: Player is null.");
                throw new CustomArgumentNullException("PlayerRepository", nameof(player.PlayerGuid));
            }

            // Should we check if the player already exists? // user is unique
            _dbContext.Players.Add(player);
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug("PlayerRepository: Finished creating a new player with name: {PlayerName}", player.PlayerName);
        }

        public async Task DeletePlayerAsync(Player player)
        {
            _logger.LogDebug("PlayerRepository: Start deleting player with GUID: {Guid}", player.PlayerGuid);

            if (player == null)
            {
                _logger.LogWarning("PlayerRepository: Player is null.");
                throw new CustomArgumentNullException("PlayerRepository", nameof(player.PlayerGuid));
            }

            _dbContext.Players.Remove(player);
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug("PlayerRepository: Finished deleting player with GUID: {Guid}", player.PlayerGuid);
        }

        public async Task<IEnumerable<Player>> GetAllPlayersAsync()
        {
            _logger.LogDebug("PlayerRepository: Start getting all players.");

            var players = await _dbContext.Players.ToListAsync();

            _logger.LogDebug("PlayerRepository: Finished getting all players.");
            return players;
        }

        public async Task<Player> GetPlayerByGuidAsync(Guid guid)
        {
            _logger.LogDebug("PlayerRepository: Start getting player with GUID: {Guid}", guid);

            if (guid == null)
            {
                _logger.LogWarning("PlayerRepository: GUID is null.");
                throw new CustomArgumentNullException("PlayerRepository", nameof(guid));
            }

            var player = await _dbContext.Players.FirstOrDefaultAsync(p => p.PlayerGuid == guid);
            if (player == null)
            {
                _logger.LogWarning("PlayerRepository: No player found with GUID: {Guid}", guid);
                throw new EntityNotFoundException("PlayerRepository", nameof(Player), guid);
            }

            _logger.LogDebug("PlayerRepository: Finished getting player with GUID: {Guid}", guid);
            return player;
        }

        public async Task<IEnumerable<Player>> GetPlayersByTeamAsync(Team team)
        {
            _logger.LogDebug("PlayerRepository: Start getting players by team.");

            var players = await _dbContext.Players
                                          .Where(p => p.Teams.Any(t => t.RoleBearerGuid == team.Guid))
                                          .ToListAsync();

            if (players == null)
            {
                _logger.LogWarning("PlayerRepository: No players found for team: {Team}", team.OrgUnitName);
                throw new EntityNotFoundException("PlayerRepository", nameof(Player), team.Guid);
            }

            _logger.LogDebug("PlayerRepository: Finished getting players by team.");
            return players;
        }

        public async Task UpdatePlayerAsync(Player player)
        {
            _logger.LogDebug("PlayerRepository: Start updating player with GUID: {Guid}", player.PlayerGuid);

            if (player == null)
            {
                _logger.LogWarning("PlayerRepository: Player is null.");
                throw new CustomArgumentNullException("PlayerRepository", nameof(player.PlayerGuid));
            }

            _dbContext.Players.Update(player);
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug("PlayerRepository: Finished updating player with GUID: {Guid}", player.PlayerGuid);
        }
    }
}
