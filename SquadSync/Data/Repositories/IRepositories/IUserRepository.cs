using SquadSync.Data.Models;

namespace SquadSync.Data.Repositories.IRepositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserModel>> GetAllUsersAync();
        Task<UserModel> GetUserByGuidAsync(Guid guid);
        Task<UserModel> GetUserByEmailNormalizedAsync(string emailNormalized);
        Task AddUserAsync(UserModel user);
        Task UpdateUserAsync(UserModel user);
        Task DeleteUserAsync(Guid guid);
    }
}
