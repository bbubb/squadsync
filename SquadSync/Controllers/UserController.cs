using Microsoft.AspNetCore.Mvc;
using SquadSync.DTOs.Responses;
using SquadSync.Services.IServices;
using SquadSync.Utilities.IUtilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SquadSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEmailValidationService _emailValidationUtilityService;
        private readonly ILogger<UserController> _logger;

        // Constructor to inject services and logger
        public UserController(
            IUserService userService,
            IEmailValidationService emailValidationUtilityService,
            ILogger<UserController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _emailValidationUtilityService = emailValidationUtilityService ?? throw new ArgumentNullException(nameof(emailValidationUtilityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAllUsers()
        {
            _logger.LogInformation("UserController: Getting all users");
            var users = await _userService.GetAllUserDtosAsync();
            return Ok(users);
        }

        // GET: api/users/{guid}
        [HttpGet("{guid}")]
        public async Task<ActionResult<UserResponseDto>> GetUserByGuid(Guid guid)
        {
            _logger.LogInformation("UserController: Getting user with GUID: {Guid}", guid);
            var user = await _userService.GetUserDtoByGuidAsync(guid);
            return user is not null ? Ok(user) : NotFound($"User with GUID {guid} not found");
        }

        // GET: api/users/email/{email}
        [HttpGet("email/{email}")]
        public async Task<ActionResult<UserResponseDto>> GetUserByEmail(string email)
        {
            if (!_emailValidationUtilityService.IsValidEmail(email))
            {
                _logger.LogWarning("UserController: Invalid email format for email: {Email}", email);
                return BadRequest("Invalid email format");
            }

            _logger.LogInformation("UserController: Getting user with email: {Email}", email);
            var user = await _userService.GetUserDtoByEmailNormalizedAsync(email);
            return user is not null ? Ok(user) : NotFound($"User with email {email} not found");
        }
    }
}