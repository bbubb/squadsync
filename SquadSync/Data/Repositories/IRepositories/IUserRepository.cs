using SquadSync.Data.Models;

namespace SquadSync.Data.Repositories.IRepositories
{
    public interface IUserRepository
    {
        Task ArchiveUserAsync(User user);
        Task CreateUserAsync(User user);
        Task DeleteUserAsync(User user);
        Task<IEnumerable<User>> GetAllActiveUsersAsync();
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByEmailNormalizedAsync(string emailNormalized);
        Task<User> GetUserByGuidAsync(Guid guid);
        Task UpdateUserAsync(User user);
    }
}
