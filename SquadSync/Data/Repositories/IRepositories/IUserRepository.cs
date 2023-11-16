using SquadSync.Data.Models;

namespace SquadSync.Data.Repositories.IRepositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<IEnumerable<User>> GetAllActiveUsersAsync();
        Task<User> GetUserByGuidAsync(Guid guid);
        Task<User> GetUserByEmailNormalizedAsync(string emailNormalized);
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task ArchiveUserAsync(User user);
        Task DeleteUserAsync(User user);
    }
}
