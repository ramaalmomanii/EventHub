using EventHub.Core.DTOs.Users;
using EventHub.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using EventHub.Core.DTOs;
using Microsoft.AspNetCore.Authorization;
using EventHub.Core.Exceptions;



namespace EventHub.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserController:ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        // post :api/user/register
        [HttpPost("register")]
        public async Task<ActionResult<UserReadDto>> Register ([FromBody] UserCreateDto dto)
        {

            try
            {
                var user = await _userService.RegisterAsync(dto);
                return Ok(user);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // post: api/user/login
        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginDto dto)
        {
            try
            {
                var tokenResponse = await _userService.LoginAsync(dto.Email, dto.Password);
                if (tokenResponse == null)
                    return Unauthorized(new { message = "Invalid email or password" });

                return Ok(tokenResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // post: api/user/refresh-token
        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            try
            {
                var tokenResponse = await _userService.RefreshTokenAsync(dto.RefreshToken);
                return Ok(tokenResponse);
            }
            catch (ApplicationException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        // get: api/user/me
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var email = User.Identity?.Name;
            if (email == null)
                return Unauthorized();

            var user = await _userService.GetByEmailAsync(email);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // GET: api/user
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }



        // post :api/user/email/{email}
        [Authorize(Roles = "Admin")]
        [HttpGet("email/{email}")]
        public async Task<ActionResult<UserReadDto>>GetByEmail(string email)
        {
            var user = await _userService.GetByEmailAsync(email);
            if (user== null)
                return NotFound();
            return Ok(user);
        }
        // get :api/user/role/{role}
        [Authorize(Roles = "Admin")]
        [HttpGet("role/{role}")]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetByRole(string role)
        {
            var users = await _userService.GetByRoleAsync(role);
            return Ok(users);
        }



        [HttpPost("request-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] string email)
        {
            try
            {
                await _userService.RequestPasswordResetAsync(email);
                return Ok(new { message = "Password reset link has been sent to your email" });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            try
            {
                await _userService.ResetPasswordAsync(dto.Token, dto.NewPassword);
                return Ok(new { message = "Password has been reset successfully" });
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("send-verification/{email}")]
        public async Task<IActionResult> SendVerification(string email)
        {
            try
            {
                var token = await _userService.GenerateEmailVerificationTokenAsync(email);
                // TODO: Send email with verification link
                return Ok(new { message = "Verification link has been sent to your email" });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("verify")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            try
            {
                await _userService.VerifyEmailAsync(token);
                return Ok(new { message = "Email verified successfully" });
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }





    }

}
