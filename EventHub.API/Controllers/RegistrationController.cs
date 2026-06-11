using EventHub.Core.Constants;
using EventHub.Core.DTOs.Registertions;
using EventHub.Core.DTOs.Ticket;
using EventHub.Core.Entities;
using EventHub.Core.Exceptions;
using EventHub.Core.Interfaces.Services;
using EventHub.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;

        public RegistrationController(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<RegistrationReadDto>> Register([FromBody] RegistrationCreateDto dto)
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            var registration = await _registrationService.RegisterAsync(dto, userId);
            return Ok(registration);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;
            await _registrationService.CancelRegistrationAsync(id, userId, role);
            return NoContent();
        }

        [Authorize]
        [HttpGet("my-registrations")]
        public async Task<ActionResult<IEnumerable<RegistrationReadDto>>> GetMyRegistrations()
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            return Ok(await _registrationService.GetRegistrationsByUserAsync(userId));
        }

        [Authorize(Roles = $"{Permissions.Admin},{Permissions.Organizer}")]
        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<IEnumerable<RegistrationReadDto>>> GetByEvent(int eventId)
        {
            return Ok(await _registrationService.GetByEventAsync(eventId));
        }

        [Authorize]
        [HttpGet("user/{userId}/event/{eventId}")]
        public async Task<ActionResult<RegistrationReadDto>> GetByUserAndEvent(int userId, int eventId)
        {
            var result = await _registrationService.GetByUserAndEventAsync(userId, eventId);
            if (result == null)
                return NotFound(new { message = "Registration not found" });
            return Ok(result);
        }
    }
}
