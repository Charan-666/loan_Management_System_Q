using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Database.EntitiesDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kanini.LMP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _userService;

        public UserController(IUser userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyList<UserDTO>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<UserDTO>> CreateUser(UserDTO userDto)
        {
            try
            {
                var created = await _userService.CreateUserAsync(userDto);
                return CreatedAtAction(nameof(GetUser), new { id = created.UserId }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var success = await _userService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
                
                if (!success)
                    return BadRequest("Current password is incorrect");
                    
                return Ok(new { message = "Password changed successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
    }
}