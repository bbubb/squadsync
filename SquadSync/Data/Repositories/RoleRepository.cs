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

        public async Task<Role> GetRoleByGuidAsync (Guid roleGuid)
        {
            _logger.LogDebug("RoleRepository: Retrieving role by GUID: {Guid}", roleGuid);
            var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Guid == roleGuid);
            if (role == null)
            {
                throw new EntityNotFoundException("RoleRepository", nameof(role), roleGuid);
            }
            return role;
        }
        public async Task AssignRoleToUserAsync(Guid userGuid, RoleCreateRequestDto roleCreateRequestDto)
        {
            _logger.LogDebug("RoleRepository: Start assigning role to user with GUID: {userGuid}", userGuid);

            // Get the User and Team from the database
            var user = await _userRepository.GetUserByGuidAsync(userGuid);
            var team = await _dbContext.Teams.FirstOrDefaultAsync(t => t.Guid == roleCreateRequestDto.TeamGuid);
            // convert to a get team by guid method in team repo or add null check here

            // Create a new Role instance
            var role = new Role
            {
                RoleName = Enum.Parse<RoleNameEnum>(roleCreateRequestDto.RoleName),
                UserId = user.UserId,
                User = user,
                TeamId = team.TeamId,
                Team = team,
                RoleStatus = RoleStatusEnum.Pending,
                CreatedOn = DateTime.UtcNow
            };

            // Add Roll to DbContext
            _dbContext.Roles.Add(role);


            // Create a new RoleRequest instance for this role
            var roleRequest = new RoleRequest
            {
                RoleId = role.RoleId,
                Role = role,
                UserId = user.UserId,
                User = user,
                TeamId = team.TeamId,
                Team = team,
                Status = RoleRequestStatusEnum.Pending,
                RequestedOn = DateTime.UtcNow
            };

            // Add RoleRequest to DbContext
            _dbContext.RoleRequests.Add(roleRequest);

            // Update the RoleRequest property on the Role instance
            role.RoleRequestId = roleRequest.RoleRequestId;
            role.RoleRequest = roleRequest;
            
            _logger.LogDebug("RoleRepository: Saving assigned role to the database.");
            // Save changes to the database
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {  
            _logger.LogDebug("RoleRepository: Retrieving all roles from the database.");
            return await _dbContext.Roles.ToListAsync();
        }

        public async Task<IEnumerable<Role>> GetRolesByUserGuidAsync(Guid userGuid)
        {
            var user = await _userRepository.GetUserByGuidAsync(userGuid);

            _logger.LogDebug("RoleRepository: Retrieving role by user GUID: {Guid}", userGuid);
            return user.Roles;
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(RoleNameEnum role)
        {
            _logger.LogDebug("RoleRepository: Retrieving users by role: {Role}", role);

            var users = await _dbContext.Users.Include(u => u.Roles).Where(u => u.Roles.Any(r => r.RoleName == role)).ToListAsync();
            if (users == null)
            {
                _logger.LogWarning("RoleRepository: No users found with role: {Role}", role);
                throw new EntityNotFoundException("RoleRepository", nameof(users), role);
            }
            return users;
        }

        public async Task<IEnumerable<User>> GetUserWithMultipleRolesAsync(RoleNameEnum[] roles)
        {
            _logger.LogDebug("RoleRepository: Retrieving users with multiple roles: {Roles}", roles);

            var users = await _dbContext.Users.Include(u => u.Roles).Where(u => u.Roles.Any(r => roles.Contains(r.RoleName))).ToListAsync();
            if (users == null)
            {
                _logger.LogWarning("RoleRepository: No users found with roles: {Roles}", roles);
                throw new EntityNotFoundException("RoleRepository", nameof(users), roles);
            }
            return users;
        }

        public async Task RemoveRoleFromUserAsync(Guid userGuid, Guid roleGuid)
        {
            _logger.LogDebug("RoleRepository: Start removing role with GUID: {roleGuid} from user with GUID: {userGuid}", roleGuid, userGuid);

            // Get the Role from the database
            var role = await _dbContext.Roles
                                       .Include(r => r.RoleRequest) // Include the RoleRequest in the query
                                       .FirstOrDefaultAsync(r => r.Guid == roleGuid && r.User.Guid == userGuid);

            if (role == null)
            {
                throw new EntityNotFoundException("RoleRepository", nameof(role), roleGuid);
            }

            // Soft delete (archive) the role
            role.ArchivedOn = DateTime.UtcNow;
            role.RoleStatus = RoleStatusEnum.Cancelled;

            // Soft delete (archive) the associated RoleRequest if it exists
            if (role.RoleRequest != null)
            {
                role.RoleRequest.ArchivedOn = DateTime.UtcNow;
                role.RoleRequest.Status = RoleRequestStatusEnum.Cancelled;
            }

            // Save changes to the database
            _logger.LogDebug("RoleRepository: Finish removing role with GUID: {roleGuid} from user with GUID: {userGuid}", roleGuid, userGuid);
            await _dbContext.SaveChangesAsync();
        }

    }
}
