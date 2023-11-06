using SquadSync.Data.Models;

namespace SquadSync.Data.Repositories.IRepositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAync();
        Task<User> GetUserByGuidAsync(Guid guid);
        Task<User> GetUserByEmailNormalizedAsync(string emailNormalized);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(Guid guid);
    }
}
