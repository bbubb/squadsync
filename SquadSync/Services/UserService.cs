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
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepository,
            IMapper mapper,
            IEmailValidationUtilityService emailValidationUtility,
            IEmailNormalizationUtilityService emailNormalizationUtility,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _emailValidationUtility = emailValidationUtility;
            _emailNormalizationUtility = emailNormalizationUtility;
            _logger = logger;
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUserDtosAsync()
        {
            _logger.LogInformation("UserService: Retrieving all user DTOs.");
            var users = await _userRepository.GetAllUsersAsync();
            return _mapper.Map<IEnumerable<UserResponseDto>>(users);
        }

        public async Task<UserResponseDto> GetUserDtoByEmailNormalizedAsync(string email)
        {
            _logger.LogInformation("UserService: Validating and normalizing email: {Email}", email);

            // Validation
            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogError("UserService: Email cannot be empty.");
                throw new ArgumentException("Email cannot be empty.");
            }

            if (!_emailValidationUtility.IsValidEmail(email))
            {
                _logger.LogError("UserService: Invalid email format for email: {Email}", email);
                throw new ArgumentException("Invalid email format.");
            }

            // Normalization
            var normalizedEmail = _emailNormalizationUtility.NormalizeEmail(email);

            // Query
            _logger.LogInformation("UserService: Retrieving user by normalized email: {NormalizedEmail}", normalizedEmail);
            var user = await _userRepository.GetUserByEmailNormalizedAsync(normalizedEmail);
            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserResponseDto> GetUserDtoByGuidAsync(Guid guid)
        {
            _logger.LogInformation("UserService: Retrieving user by GUID: {Guid}", guid);
            var user = await _userRepository.GetUserByGuidAsync(guid);
            return _mapper.Map<UserResponseDto>(user);
        }
    }
}

