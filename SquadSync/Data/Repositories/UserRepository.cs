using SquadSync.Data.Models;
using Microsoft.EntityFrameworkCore;
using SquadSync.Exceptions;
using SquadSync.Data.Repositories.IRepositories;
using SquadSync.Enums;
using SquadSync.DTOs.Requests;

namespace SquadSync.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SQLDbContext _dbContext;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(
            SQLDbContext dbContext,
            ILogger<UserRepository> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            _logger.LogDebug("UserRepository: Retrieving all users from the database.");
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllActiveUsersAsync()
        {
            _logger.LogDebug("UserRepository: Retrieving all active users from the database.");
            return await _dbContext.Users
                .Where(u => u.UserStatus != UserStatusEnum.RegisteredInactive && u.UserStatus != UserStatusEnum.RegisteredArchived)
                .ToListAsync();
        }

        public async Task<User> GetUserByGuidAsync(Guid guid)
        {
            _logger.LogDebug("UserRepository: Retrieving user by GUID: {Guid}", guid);
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Guid == guid);
            if (user == null)
            {
                _logger.LogWarning("UserRepository: No user found with GUID: {Guid}", guid);
                throw new EntityNotFoundException("User", guid);
            }
            return user;
        }

        public async Task<User> GetUserByEmailNormalizedAsync(string emailNormalized)
        {
            _logger.LogDebug("UserRepository: Retrieving user by normalized email: {EmailNormalized}", emailNormalized);
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.EmailNormalized == emailNormalized);
            if (user == null)
            {
                _logger.LogWarning("UserRepository: No user found with the normalized email: {EmailNormalized}", emailNormalized);
                throw new EntityNotFoundException("User", emailNormalized);
            }
            return user;
        }

        public async Task CreateUserAsync(User user)
        {
            if (user == null)
            {
                _logger.LogWarning("UserRepository: Attempted to create a user with a null entity.");
                throw new ArgumentNullException(nameof(user));
            }

            _logger.LogInformation("UserRepository: Creating a new user with email: {Email}", user.Email);
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(Guid userGuid, UserUpdateRequestDto userUpdateRequestDto)
        {
            var user = await GetUserByGuidAsync(userGuid);
            
            // Implement UserUpdateRequestDto Logic Here

            _logger.LogInformation("UserRepository: Updating user with GUID: {Guid}", user.Guid);
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(Guid guid)
        {
            var user = await GetUserByGuidAsync(guid);

            _logger.LogDebug("UserRepository: Deleting user with GUID: {Guid}", guid);
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}
