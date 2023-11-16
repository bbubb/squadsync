using AutoMapper;
using SquadSync.Data.Models;
using SquadSync.Data.Repositories.IRepositories;
using SquadSync.DTOs;
using SquadSync.DTOs.Requests;
using SquadSync.DTOs.Responses;
using SquadSync.Enums;
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

        public async Task<ServiceResult<UserResponseDto>> UpdateUserByGuidAsync(Guid guid, UserUpdateRequestDto dto)
        {
            _logger.LogInformation("UserService: Updating user with GUID: {Guid}", guid);

            var user = await _userRepository.GetUserByGuidAsync(guid);

            // Apply changes from dto to user
            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;
            if (dto.Email != null)
            {
                if (!_emailValidationUtility.IsValidEmail(dto.Email))
                {
                    _logger.LogError("UserService: Invalid email format for email: {Email}", dto.Email);
                    return ServiceResult<UserResponseDto>.Failure("Invalid email format.");
                }
                user.Email = dto.Email ?? user.Email;
                user.EmailNormalized = _emailNormalizationUtility.NormalizeEmail(dto.Email);
            }
            user.DateOfBirth = dto.DateOfBirth ?? user.DateOfBirth;
            user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;
            user.UserStatus = user.UserStatus;
            user.CreatedOn = user.CreatedOn;
            // Insert UpdatedOn current DateTime
            user.UpdatedOn = DateTime.UtcNow;
            user.ArchivedOn = user.ArchivedOn;

            await _userRepository.UpdateUserAsync(user);

            // Return updated user dto
            _logger.LogInformation("UserService: Finished updating user with GUID: {Guid}", guid);
            return ServiceResult<UserResponseDto>.SuccessResult(_mapper.Map<UserResponseDto>(user));
        }

        public async Task<ServiceResult<UserResponseDto>> ArchiveUserByGuidAsync(Guid guid)
        {
            _logger.LogInformation("UserService: Start archiving user with GUID: {Guid}", guid);

            var user = await _userRepository.GetUserByGuidAsync(guid);

            user.ArchivedOn = DateTime.UtcNow;
            user.UserStatus = UserStatusEnum.RegisteredArchived;

            await _userRepository.UpdateUserAsync(user);

            _logger.LogInformation("UserService: Finished archiving user with GUID: {Guid}", guid);
            return ServiceResult<UserResponseDto>.SuccessResult(_mapper.Map<UserResponseDto>(user));
        }

        public async Task<ServiceResult<UserResponseDto>> CreateUserAsync(UserCreateRequestDto dto)
        {
            _logger.LogInformation("UserService: Start creating a new user from dto");

            // Validation
            if (!_emailValidationUtility.IsValidEmail(dto.Email))
            {
                _logger.LogError("UserService: Invalid email format for email: {Email}", dto.Email);
                return ServiceResult<UserResponseDto>.Failure("Invalid email format.");
            }

            var user = new User
            {
                Guid = Guid.NewGuid(),
                UserName = dto.UserName,
                Email = dto.Email,
                EmailNormalized = _emailNormalizationUtility.NormalizeEmail(dto.Email),
                FirstName = dto.FirstName ?? null,
                LastName = dto.LastName ?? null,
                DateOfBirth = dto.DateOfBirth ?? null,
                PhoneNumber = dto.PhoneNumber ?? null,
                UserStatus = UserStatusEnum.RegisteredPending,  // Should this be handled differently?
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = null,
                ArchivedOn = null
            };

            await _userRepository.CreateUserAsync(user);

            _logger.LogInformation("UserService: Finished creating a new user from dto");
            return ServiceResult<UserResponseDto>.SuccessResult(_mapper.Map<UserResponseDto>(user));
        }

        public async Task<ServiceResult> DeleteUserByGuidAsync(Guid guid)
        {
            try
            {
                var user = await _userRepository.GetUserByGuidAsync(guid);
                if (user == null)
                {
                    return ServiceResult.Failure("User not found.");
                }

                await _userRepository.DeleteUserAsync(user);
                return ServiceResult.SuccessResult();
            }
            catch (Exception ex)
            {
                _logger.LogError("UserService: An error occurred while processing your request: {Message}", ex.Message);
                return ServiceResult.Failure("An error occurred while processing your request.");
            }
        }
    }
}

