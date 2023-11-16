using SquadSync.DTOs.Responses;
using SquadSync.DTOs;
using SquadSync.Utilities;
using SquadSync.DTOs.Requests;

namespace SquadSync.Services.IServices
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllUserDtosAsync();
        Task<ServiceResult<UserResponseDto>> GetUserDtoByGuidAsync(Guid guid);
        Task<ServiceResult<UserResponseDto>> GetUserDtoByEmailNormalizedAsync(string email);
        Task<ServiceResult<UserResponseDto>> CreateUserAsync(UserCreateRequestDto dto);
        Task<ServiceResult<UserResponseDto>> UpdateUserByGuidAsync(Guid guid, UserUpdateRequestDto dto);
        Task<ServiceResult<UserResponseDto>> ArchiveUserByGuidAsync(Guid guid);
        Task<ServiceResult> DeleteUserByGuidAsync(Guid guid);

    }
}
