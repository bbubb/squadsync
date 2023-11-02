using SquadSync.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.Serialization;
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

        public async Task<IEnumerable<UserModel>> GetAllUsersAync()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<UserModel> GetUserByGuidAsync(Guid guid)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Guid == guid)
                ?? throw new UserNotFoundException($"No user found with the GUID '{guid}'.");
        }

        public async Task<UserModel> GetUserByEmailNormalizedAsync(string emailNormalized)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.EmailNormalized == emailNormalized)
                ?? throw new UserNotFoundException($"No user found with the email '{emailNormalized}'.");
        }

        public async Task AddUserAsync(UserModel user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(UserModel user)
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
