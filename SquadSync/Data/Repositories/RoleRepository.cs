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
        UserRepository _userRepository;
        ILogger<RoleRepository> _logger;

        public RoleRepository(SQLDbContext dbContext, UserRepository userRepository,ILogger<RoleRepository> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
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

        public async Task<Role> GetRoleByGuidAsync (Guid roleGuid)
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

        public async Task<IEnumerable<Role>> GetRolesByUserGuidAsync(Guid userGuid)
        {
            _logger.LogDebug("RoleRepository: Retrieving roles by user GUID: {Guid}", userGuid);

            var roles = await _dbContext.Users.Where(u => u.Guid == userGuid).Select(u => u.Roles).FirstOrDefaultAsync();
            if (roles == null)
            {
                _logger.LogWarning("RoleRepository: Roles for user with GUID {Guid} not found", userGuid);
                throw new EntityNotFoundException("RoleRepository", nameof(roles), userGuid);
            }
            return roles;
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(RoleNameEnum role)
        {
            _logger.LogDebug("RoleRepository: Retrieving users by role type: {RoleNameEnum}", role);

            var users = await _dbContext.Users.Where(u => u.Roles.Any(r => r.RoleName == role)).ToListAsync();
            if (users == null)
            {
                _logger.LogWarning("RoleRepository: Users with role type {RoleNameEnum} not found", role);
                throw new EntityNotFoundException("RoleRepository", nameof(users), role);
            }
            return users;
        }

        public async Task<IEnumerable<User>> GetUserWithMultipleRolesAsync(RoleNameEnum[] roles)
        {
            _logger.LogDebug("RoleRepository: Retrieving users with multiple roles");

            var users = await _dbContext.Users.Where(u => u.Roles.Any(r => roles.Contains(r.RoleName))).ToListAsync();
            if (users == null)
            {
                _logger.LogWarning("RoleRepository: Users with multiple roles not found");
                throw new EntityNotFoundException("RoleRepository", nameof(users), roles);
            }
            return users;
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
