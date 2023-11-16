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

        public async Task ArchiveTeamAsync(Team team)
        {
            _logger.LogDebug("TeamRepository: Start archiving team with GUID: {Guid}", team.Guid);

            if (team == null)
            {
                _logger.LogWarning("TeamRepository: Team is null.");
                throw new CustomArgumentNullException("TeamRepository", nameof(team.Guid));
            }

            _dbContext.Teams.Update(team);
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug("TeamRepository: Finished archiving team with GUID: {Guid}", team.Guid);
        }

        public async Task CreateTeamAsync(Team team)
        {
            _logger.LogDebug("TeamRepository: Start creating team with GUID: {Guid}", team.Guid);

            if (team == null)
            {
                _logger.LogWarning("TeamRepository: Team is null.");
                throw new CustomArgumentNullException("TeamRepository", nameof(team.Guid));
            }

            _dbContext.Teams.Add(team);
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug("TeamRepository: Finished creating team with GUID: {Guid}", team.Guid);
        }

        public async Task DeleteTeamAsync(Team team)
        {
            _logger.LogDebug("TeamRepository: Start deleting team with GUID: {Guid}", team.Guid);

            if (team == null)
            {
                _logger.LogWarning("TeamRepository: Team is null.");
                throw new CustomArgumentNullException("TeamRepository", nameof(team.Guid));
            }

            _dbContext.Teams.Remove(team);
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug("TeamRepository: Finished deleting team with GUID: {Guid}", team.Guid);
        }

        public async Task<IEnumerable<Team>> GetAllActiveTeamsAsync()
        {
            _logger.LogDebug("TeamRepository: Retrieving all active teams.");

            return await _dbContext.Teams
                .Where(t => new[] {TeamStatusEnum.UnregisteredActive, TeamStatusEnum.RegisteredActive}.Contains(t.TeamStatus))
                .ToListAsync();
        }

        public async Task<IEnumerable<Team>> GetAllTeamsAsync()
        {
            _logger.LogDebug("TeamRepository: Retrieving all teams.");

            return await _dbContext.Teams.ToListAsync();
        }

        public async Task<Team> GetTeamByGuidAsync(Guid guid)
        {
            _logger.LogDebug("TeamRepository: Retrieving team with GUID: {Guid}", guid);

            var team = await _dbContext.Teams
                .FirstOrDefaultAsync(t => t.Guid == guid);
            if (team == null)
            {
                _logger.LogWarning("TeamRepository: No team found with GUID: {Guid}", guid);
                throw new EntityNotFoundException("TeamRepository", nameof(Team), guid);
            }
            return team;
        }

        public async Task<IEnumerable<Team>> GetTeamsByNameAsync(string name)
        {
            _logger.LogDebug("TeamRepository: Retrieving teams with name: {Name}", name);

            var teams = await _dbContext.Teams
                .Where(t => t.TeamName == name)
                .ToListAsync();
            if (teams.IsNullOrEmpty())
            {
                _logger.LogWarning("TeamRepository: No teams found with name: {Name}", name);
                throw new EntityNotFoundException("TeamRepository", nameof(Team), name);
            }
            return teams;
        }

        public async Task UpdateTeamAsync(Team team)
        {
            _logger.LogDebug("TeamRepository: Start updating team with GUID: {Guid}", team.Guid);

            if (team == null)
            {
                _logger.LogWarning("TeamRepository: Team is null.");
                throw new CustomArgumentNullException("TeamRepository", nameof(team.Guid));
            }

            _dbContext.Teams.Update(team);
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug("TeamRepository: Finished updating team with GUID: {Guid}", team.Guid);
        }
    }
}
