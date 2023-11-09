using SquadSync.DTOs.Responses;
using SquadSync.DTOs;
using SquadSync.Utilities;

namespace SquadSync.Services.IServices
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllUserDtosAsync();
        Task<ServiceResult<UserResponseDto>> GetUserDtoByGuidAsync(Guid guid);
        Task<ServiceResult<UserResponseDto>> GetUserDtoByEmailNormalizedAsync(string email);
    }
}
