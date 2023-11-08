using Microsoft.AspNetCore.Mvc;
using SquadSync.DTOs.Responses;
using SquadSync.Services.IServices;

namespace SquadSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<UsersAllResponseDto>> GetAllUsers()
        {
            try
            {
                _logger.LogInformation("UserController: Getting all users");
                var users = await _userService.GetAllUserDtosAsync();

                if (users == null || !users.Any())
                {
                    _logger.LogWarning("UserController: No users found.");
                    return NotFound("No users found.");
                }
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all users.");
                return StatusCode(500, "An internal server error has occurred.");
            }
        }

        // GET: api/users/{guid}
        [HttpGet("{guid}")]
        public async Task<ActionResult<UserResponseDto>> GetUserByGuid(Guid guid)
        {
            try
            {
                _logger.LogInformation("UserController: Getting user with GUID: {Guid}", guid);
                var user = await _userService.GetUserDtoByGuidAsync(guid);

                if (user == null)
                {
                    _logger.LogWarning("UserController: User with GUID {Guid} not found.", guid);
                    return NotFound($"User with GUID {guid} not found.");
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting user with GUID: {Guid}", guid);
                return StatusCode(500, "An internal server error has occurred.");
            }
        }


        // GET: api/users/email/{email}
        [HttpGet("email/{email}")]
        public async Task<ActionResult<UserResponseDto>> GetUserByEmail(string email)
        {
            try
            {
                _logger.LogInformation("UserController: Getting user with email: {Email}", email);
                var user = await _userService.GetUserDtoByEmailNormalizedAsync(email);

                if (user == null)
                {
                    _logger.LogWarning("UserController: User with email {Email} not found.", email);
                    return NotFound($"User with email {email} not found.");
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting user with email: {Email}", email);
                return StatusCode(500, "An internal server error has occurred.");
            }
        }
    }
}