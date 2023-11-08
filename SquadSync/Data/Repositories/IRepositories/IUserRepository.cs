using SquadSync.Data.Models;

namespace SquadSync.Data.Repositories.IRepositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByGuidAsync(Guid guid);
        Task<User> GetUserByEmailNormalizedAsync(string emailNormalized);
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(Guid guid);
        Task DeleteUserAsync(Guid guid);
    }
}
