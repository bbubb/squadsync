using AutoMapper;
using SquadSync.Data.Repositories.IRepositories;
using SquadSync.DTOs;
using SquadSync.DTOs.Responses;
using SquadSync.Services.IServices;
using SquadSync.Utilities;
using SquadSync.Utilities.IUtilities;

namespace SquadSync.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IEmailValidationService _emailValidationUtility;
        private readonly IEmailNormalizationService _emailNormalizationUtility;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepository,
            IMapper mapper,
            IEmailValidationService emailValidationUtility,
            IEmailNormalizationService emailNormalizationUtility,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _emailValidationUtility = emailValidationUtility ?? throw new ArgumentNullException(nameof(emailValidationUtility));
            _emailNormalizationUtility = emailNormalizationUtility ?? throw new ArgumentNullException(nameof(emailNormalizationUtility));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUserDtosAsync()
        {
            _logger.LogInformation("UserService: Retrieving all user DTOs.");
            var users = await _userRepository.GetAllUsersAsync();
            return _mapper.Map<IEnumerable<UserResponseDto>>(users);
        }

        public async Task<ServiceResult<UserResponseDto>> GetUserDtoByEmailNormalizedAsync(string email)
        {
            _logger.LogInformation("UserService: Validating and normalizing email: {Email}", email);

            // Validation
            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogError("UserService: Email cannot be empty.");
                return ServiceResult<UserResponseDto>.Failure("Email cannot be empty.");
            }

            if (!_emailValidationUtility.IsValidEmail(email))
            {
                _logger.LogError("UserService: Invalid email format for email: {Email}", email);
                return ServiceResult<UserResponseDto>.Failure("Invalid email format.");
            }

            // Normalization
            var normalizedEmail = _emailNormalizationUtility.NormalizeEmail(email);
            _logger.LogInformation("UserService: Retrieving user by normalized email: {NormalizedEmail}", normalizedEmail);

            // Query
            var user = await _userRepository.GetUserByEmailNormalizedAsync(normalizedEmail);
            if (user == null)
            {
                _logger.LogWarning("UserService: User with email {Email} not found.", email);
                return ServiceResult<UserResponseDto>.Failure($"User with email {email} not found.");
            }

            return ServiceResult<UserResponseDto>.SuccessResult(_mapper.Map<UserResponseDto>(user));
        }

        public async Task<ServiceResult<UserResponseDto>> GetUserDtoByGuidAsync(Guid guid)
        {
            _logger.LogInformation("UserService: Retrieving user by GUID: {Guid}", guid);

            var user = await _userRepository.GetUserByGuidAsync(guid);
            if (user == null)
            {
                _logger.LogWarning("UserService: User with GUID {Guid} not found.", guid);
                return ServiceResult<UserResponseDto>.Failure($"User with GUID {guid} not found.");
            }

            return ServiceResult<UserResponseDto>.SuccessResult(_mapper.Map<UserResponseDto>(user));
        }
    }
}

