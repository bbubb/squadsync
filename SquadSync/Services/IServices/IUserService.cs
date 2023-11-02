using SquadSync.DTOs;

namespace SquadSync.Services.IServices
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUserDtosAsync();
        Task<UserDto> GetUserDtoByGuidAsync(Guid guid);
        Task<UserDto> GetUserDtoByEmailNormalizedAsync(string email);
    }
}
