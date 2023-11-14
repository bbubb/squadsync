﻿using SquadSync.Data.Models;
using SquadSync.DTOs.Requests;

namespace SquadSync.Data.Repositories.IRepositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<IEnumerable<User>> GetAllActiveUsersAsync();
        Task<User> GetUserByGuidAsync(Guid guid);
        Task<User> GetUserByEmailNormalizedAsync(string emailNormalized);
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(Guid guid, UserUpdateRequestDto dto);
        Task DeleteUserAsync(Guid guid);
    }
}
