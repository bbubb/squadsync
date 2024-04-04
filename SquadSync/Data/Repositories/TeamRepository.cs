using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SquadSync.Data.Models;
using SquadSync.Data.Repositories.IRepositories;
using SquadSync.Enums;
using SquadSync.Exceptions;

namespace SquadSync.Data.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly SQLDbContext _dbContext;
        private readonly ILogger<TeamRepository> _logger;

        public TeamRepository(
            SQLDbContext dbContext,
            ILogger<TeamRepository> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Team>> GetTeamsByPlayers(IEnumerable<Player> players)
        {
            _logger.LogDebug("TeamRepository: Start getting teams by players.");
            var teams = new List<Team>();

            foreach (var player in players)
            {
                var team = await _dbContext.Teams
                    .Include(t => t.PlayerRoster)
                    .FirstOrDefaultAsync(t => t.PlayerRoster.Contains(player));

                if (team == null)
                {
                    throw new EntityNotFoundException("TeamRepository", nameof(Team), player.PlayerGuid);
                }

                teams.Add(team);
            }

            _logger.LogDebug("TeamRepository: Finished getting teams by players.");
            return teams;
        }
    }
}
