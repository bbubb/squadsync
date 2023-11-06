using SquadSync.Data.Models;
using Microsoft.EntityFrameworkCore;
using SquadSync.Exceptions;
using SquadSync.Data.Repositories.IRepositories;

namespace SquadSync.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SQLDbContext _dbContext;

        public UserRepository(SQLDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<User>> GetAllUsersAync()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<User> GetUserByGuidAsync(Guid guid)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Guid == guid)
                ?? throw new UserNotFoundException($"No user found with the GUID '{guid}'.");
        }

        public async Task<User> GetUserByEmailNormalizedAsync(string emailNormalized)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.EmailNormalized == emailNormalized)
                ?? throw new UserNotFoundException($"No user found with the email '{emailNormalized}'.");
        }

        public async Task CreateUserAsync(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(Guid guid)
        {
            var user = await GetUserByGuidAsync(guid);
            if (user == null) throw new UserNotFoundException($"No user found with the GUID '{guid}'.");

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}
