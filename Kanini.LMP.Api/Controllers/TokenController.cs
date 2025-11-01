using Kanini.LMP.Data.Repositories.Implementations;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Database.EntitiesDtos.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.LMP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {

        private readonly ITokenService _tokenService;
        private readonly IUser _userService;

        public TokenController(IUser userService, ITokenService tokenService)
        {

            _tokenService = tokenService;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
                return BadRequest("Username and password are required");

            var user = await _userService.GetByUsernameAsync(loginDto.Username);

            if (user == null)
                return Unauthorized($"Invalid username: {loginDto.Username}");


            if (user.PasswordHash != loginDto.Password)
                return Unauthorized("Invalid password");

            var token = _tokenService.GenerateToken(user);

            return Ok(new
            {
                token,
                username = user.FullName,
                role = user.Roles!
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterCustomer([FromBody] CustomerRegistrationDTO registrationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userDto = await _userService.RegisterCustomerAsync(registrationDto);
                
                return Ok(new
                {
                    message = "Customer registered successfully",
                    userId = userDto.UserId,
                    email = userDto.Email,
                    fullName = userDto.FullName
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Registration failed: {ex.Message}");
            }
        }
    }
}
