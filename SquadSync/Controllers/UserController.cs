using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SquadSync.DTOs.Responses;
using SquadSync.Services.IServices;
using System.Threading.Tasks;
using System;
using SquadSync.DTOs;

namespace SquadSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<UsersAllResponseDto>> GetAllUsers()
        {
            var users = await _userService.GetAllUserDtosAsync();
            return Ok(users);
        }

        // GET: api/users/{guid}
        [HttpGet("{guid}")]
        public async Task<ActionResult<UserDto>> GetUserByGuid(Guid guid)
        {
            var user = await _userService.GetUserDtoByGuidAsync(guid);
            if (user == null)
                return NotFound($"User with GUID {guid}  not found.");
            return Ok(user);
        }

        // GET: api/users/email/{email}
        [HttpGet("email/{email}")]
        public async Task<ActionResult<UserDto>> GetUserByEmail(string email)
        {
            var user = await _userService.GetUserDtoByEmailNormalizedAsync(email);
            if (user == null)
                return NotFound($"User with email {email} not found.");
            return Ok(user);
        }
    }
}