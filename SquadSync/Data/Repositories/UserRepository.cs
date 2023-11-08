using SquadSync.Data.Models;
using Microsoft.EntityFrameworkCore;
using SquadSync.Exceptions;
using SquadSync.Data.Repositories.IRepositories;

namespace SquadSync.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SQLDbContext _dbContext;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(SQLDbContext dbContext, ILogger<UserRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            _logger.LogDebug("UserRepository: Retrieving all users from the database.");
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<User> GetUserByGuidAsync(Guid guid)
        {
            _logger.LogDebug("UserRepository: Retrieving user by GUID: {Guid}", guid);
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Guid == guid);
            if (user == null)
            {
                _logger.LogWarning("UserRepository: No user found with GUID: {Guid}", guid);
                throw new UserNotFoundException($"No user found with the GUID '{guid}'.");
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
                throw new UserNotFoundException($"No user found with the email '{emailNormalized}'.");
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

        public async Task UpdateUserAsync(Guid guid)
        {
            var user = await GetUserByGuidAsync(guid);
            if (user == null)
            {
                _logger.LogWarning("UserRepository: Attempted to update a user with a null entity.");
                throw new ArgumentNullException(nameof(user));
            }

            _logger.LogInformation("UserRepository: Updating user with GUID: {Guid}", user.Guid);
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(Guid guid)
        {
            var user = await GetUserByGuidAsync(guid);
            if (user == null)
            {
                _logger.LogWarning("UserRepository: Attempted to delete a user that does not exist with GUID: {Guid}", guid);
                throw new UserNotFoundException($"No user found with the GUID '{guid}'.");
            }

            _logger.LogDebug("UserRepository: Deleting user with GUID: {Guid}", guid);
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}
