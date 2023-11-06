﻿using SquadSync.DTOs.Responses;
using SquadSync.DTOs;

namespace SquadSync.Services.IServices
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllUserDtosAsync();
        Task<UserDto> GetUserDtoByGuidAsync(Guid guid);
        Task<UserResponseDto> GetUserDtoByEmailNormalizedAsync(string email);
    }
}
