using EventHub.Core.Constants;
using EventHub.Core.DTOs;
using EventHub.Core.DTOs.Users;
using EventHub.Core.Exceptions;
using EventHub.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace EventHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserReadDto>> Register([FromBody] UserCreateDto dto)
        {
            var user = await _userService.RegisterAsync(dto);
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginDto dto)
        {
            var tokenResponse = await _userService.LoginAsync(dto.Email, dto.Password);
            if (tokenResponse == null)
                return Unauthorized(new { message = "Invalid email or password" });
            return Ok(tokenResponse);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            var result = await _userService.RefreshTokenAsync(dto.RefreshToken);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserReadDto>> GetMyProfile()
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            var user = await _userService.GetByIdAsync(userId);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [Authorize]
        [HttpPut("me")]
        public async Task<ActionResult<UserReadDto>> UpdateMyProfile([FromBody] UserUpdateDto dto)
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            var updated = await _userService.UpdateProfileAsync(userId, dto);
            return Ok(updated);
        }

        [Authorize(Roles = $"{Permissions.Admin}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetAll()
        {
            return Ok(await _userService.GetAllAsync());
        }

        [Authorize(Roles = $"{Permissions.Admin}")]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserReadDto>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [Authorize(Roles = $"{Permissions.Admin}")]
        [HttpGet("email/{email}")]
        public async Task<ActionResult<UserReadDto>> GetByEmail(string email)
        {
            var user = await _userService.GetByEmailAsync(email);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [Authorize(Roles = $"{Permissions.Admin}")]
        [HttpGet("role/{role}")]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetByRole(string role)
        {
            return Ok(await _userService.GetByRoleAsync(role));
        }

        [HttpPost("request-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] RequestResetDto dto)
        {
            await _userService.RequestPasswordResetAsync(dto.Email);
            return Ok(new { message = "Password reset link has been sent to your email" });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            await _userService.ResetPasswordAsync(dto.Token, dto.NewPassword);
            return Ok(new { message = "Password has been reset successfully" });
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            await _userService.ChangePasswordAsync(userId, dto);
            return Ok(new { message = "Password changed successfully" });

        }
        [HttpPost("send-verification/{email}")]
        public async Task<IActionResult> SendVerification(string email)
        {
            await _userService.GenerateEmailVerificationTokenAsync(email);
            return Ok(new { message = "Verification link has been sent to your email" });
        }

        [HttpGet("verify")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            await _userService.VerifyEmailAsync(token);
            return Ok(new { message = "Email verified successfully" });
        }

        [Authorize(Roles = $"{Permissions.Admin}")]
        [HttpPost]
        public async Task<ActionResult<UserReadDto>> CreateUser([FromBody] AdminCreateUserDto dto)
        {
            var user = await _userService.CreateUserAsync(dto);
            return Ok(user);
        }

        [Authorize(Roles = $"{Permissions.Admin}")]
        [HttpPut("{id}")]
        public async Task<ActionResult<UserReadDto>> UpdateUser(int id, [FromBody] AdminUpdateUserDto dto)
        {
            var user = await _userService.UpdateUserAsync(id, dto);
            return Ok(user);
        }

        [Authorize(Roles = $"{Permissions.Admin}")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }

    }

}
