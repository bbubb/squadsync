using AutoMapper;
using SquadSync.Data.Repositories.IRepositories;
using SquadSync.DTOs;
using SquadSync.DTOs.Responses;
using SquadSync.Services.IServices;
using SquadSync.Utilities.IUtilities;

namespace SquadSync.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IEmailValidationUtilityService _emailValidationUtility;
        private readonly IEmailNormalizationUtilityService _emailNormalizationUtility;

        public UserService(
            IUserRepository userRepository,
            IMapper mapper,
            IEmailValidationUtilityService emailValidationUtility,
            IEmailNormalizationUtilityService emailNormalizationUtility)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _emailValidationUtility = emailValidationUtility;
            _emailNormalizationUtility = emailNormalizationUtility;
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUserDtosAsync()
        {
            var users = await _userRepository.GetAllUsersAync();
            return _mapper.Map<IEnumerable<UserResponseDto>>(users);
        }

        public async Task<UserResponseDto> GetUserDtoByEmailNormalizedAsync(string email)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty.");

            if (!_emailValidationUtility.IsValidEmail(email))
                throw new ArgumentException("Invalid email format.");

            // Normalization
            var normalizedEmail = _emailNormalizationUtility.NormalizeEmail(email);

            // Query
            var user = await _userRepository.GetUserByEmailNormalizedAsync(normalizedEmail);
            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserDto> GetUserDtoByGuidAsync(Guid guid)
        {
            var user = await _userRepository.GetUserByGuidAsync(guid);
            return _mapper.Map<UserDto>(user);
        }
    }
}
