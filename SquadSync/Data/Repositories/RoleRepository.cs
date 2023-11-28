using Microsoft.EntityFrameworkCore;
using SquadSync.Data.Models;
using SquadSync.Data.Repositories.IRepositories;
using SquadSync.DTOs.Requests;
using SquadSync.Enums;
using SquadSync.Exceptions;
using SquadSync.Middleware;

namespace SquadSync.Data.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        SQLDbContext _dbContext;
        // UserRepository _userRepository;
        ILogger<RoleRepository> _logger;

        public RoleRepository(
            SQLDbContext dbContext,
            // UserRepository userRepository,
            ILogger<RoleRepository> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            // _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ArchiveRoleAsync(Role role)
        {
            _logger.LogDebug("RoleRepository: Start archiving role with GUID: {Guid}", role.Guid);

            if (role == null)
            {
                _logger.LogWarning("RoleRepository: Role is null");
                throw new CustomArgumentNullException("RoleRepository, ", nameof(role));
            }

            _dbContext.Roles.Update(role);
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug("RoleRepository: Finished archiving role with GUID: {Guid}", role.Guid);
        }

        public async Task CreateRoleAsync(Role role)
        {
            _logger.LogDebug("RoleRepository: Start creating role with GUID: {Guid}", role.Guid);

            if (role == null)
            {
                _logger.LogWarning("RoleRepository: Role is null");
                throw new CustomArgumentNullException("RoleRepository, ", nameof(role));
            }

            _logger.LogDebug("RoleRepository: Creating role with GUID: {Guid}", role.Guid);
            _dbContext.Roles.Add(role);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteRoleAsync(Role role)
        {
            _logger.LogDebug("RoleRepository: Start deleting role with GUID: {Guid}", role.Guid);

            if (role == null)
            {
                _logger.LogWarning("RoleRepository: Role is null");
                throw new CustomArgumentNullException("RoleRepository, ", nameof(role));
            }

            _dbContext.Roles.Remove(role);
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug("RoleRepository: Finished deleting role with GUID: {Guid}", role.Guid);
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            _logger.LogDebug("RoleRepository: Retrieving all roles");
            return await _dbContext.Roles.ToListAsync();
        }

        public async Task<Role> GetRoleByGuidAsync(Guid roleGuid)
        {
            _logger.LogDebug("RoleRepository: Retrieving role by GUID: {Guid}", roleGuid);

            var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Guid == roleGuid);
            if (role == null)
            {
                _logger.LogWarning("RoleRepository: Role with GUID {Guid} not found", roleGuid);
                throw new EntityNotFoundException("RoleRepository", nameof(role), roleGuid);
            }
            return role;
        }

        public async Task<IEnumerable<Role>> GetRolesByBearerGuidAsync(Guid roleBearerGuid)
        {
            _logger.LogDebug("RoleRepository: Retrieving roles by bearer GUID: {Guid}", roleBearerGuid);

            var roles = await _dbContext.Roles
                                        .Where(r => r.RoleBearer.RoleBearerGuid == roleBearerGuid)
                                        .ToListAsync();

            if (roles == null)
            {
                _logger.LogWarning("RoleRepository: Roles for bearer with GUID {Guid} not found", roleBearerGuid);
                throw new EntityNotFoundException("RoleRepository", nameof(roles), roleBearerGuid);
            }
            return roles;
        }

        public async Task<IEnumerable<IRoleBearer>> GetBearersByOURoleAsync(Role ouRole)
        {
            _logger.LogDebug("RoleRepository: Retrieving role bearers by role type: {RoleName}", ouRole.RoleName);

            var bearers = await _dbContext.Roles
                                          .Where(r => r.RoleName == ouRole.RoleName && r.RoleBearer != null)
                                          .Select(r => r.RoleBearer)
                                          .Distinct()
                                          .ToListAsync();

            if (!bearers.Any())
            {
                _logger.LogWarning("RoleRepository: Role bearers with role type {RoleName} not found", ouRole.RoleName);
                throw new EntityNotFoundException("RoleRepository", "Role bearers", ouRole.RoleName);
            }

            return bearers;
        }

        public async Task<IEnumerable<IRoleBearer>> GetBearersWithMultipleOURolesAsync(Role[] ouRoles)
        {
            _logger.LogDebug("RoleRepository: Retrieving bearers with multiple OU roles");

            // Group roles by bearer and select only those bearers that have multiple roles
            var bearersWithMultipleRoles = ouRoles.GroupBy(r => r.RoleBearer)
                                                           .Where(group => group.Count() > 1)
                                                           .Select(group => group.Key)
                                                           .ToList();

            if (!bearersWithMultipleRoles.Any())
            {
                _logger.LogWarning("RoleRepository: No bearers found with multiple roles in specified OU roles");
                throw new EntityNotFoundException("RoleRepository", "Bearers with multiple roles in specified OU roles");
            }

            return bearersWithMultipleRoles;
        }

        public async Task UpdateRoleAsync(Role role)
        {
            _logger.LogDebug("RoleRepository: Start updating role with GUID: {Guid}", role.Guid);

            if (role == null)
            {
                _logger.LogWarning("RoleRepository: Role is null");
                throw new CustomArgumentNullException("RoleRepository, ", nameof(role));
            }

            _dbContext.Roles.Update(role);
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug("RoleRepository: Finished updating role with GUID: {Guid}", role.Guid);
        }
    }
}
