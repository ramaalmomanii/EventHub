using EventHub.Core.DTOs.Registertions;
using EventHub.Core.DTOs.Ticket;
using EventHub.Core.Entities;
using EventHub.Core.Interfaces.Services;
using EventHub.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EventHub.Core.Exceptions;

namespace EventHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;
        private readonly ITicketService _ticketService; 

        public RegistrationController(IRegistrationService registrationService, ITicketService ticketService) // Add ticketService parameter
        {
            _registrationService = registrationService;
            _ticketService = ticketService; 
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Register([FromBody] RegistrationCreateDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst("id")?.Value;
                if (userIdClaim == null) 
                    return Unauthorized(new { message = "Invalid token" });

                var userId = int.Parse(userIdClaim);
                var registration = await _registrationService.RegisterAsync(dto, userId);

                var ticketDto = await _ticketService.CreateAsync(new TicketCreateDto
                {
                    RegistrationId = registration.Id,
                    EventId = registration.EventId,
                    UserId = registration.AttendeeId,
                    Price = 0,
                    SeatNumber = $"S-{registration.Id}"
                });

                return Ok(new
                {
                    Registration = registration,
                    Ticket = ticketDto 
                });
            }
            catch (ConflictException ex)
            {
                return Conflict(new { message = ex.Message });
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

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var result = await _registrationService.CancelRegistrationAsync(id);
                if (!result) 
                    return NotFound(new { message = "Registration not found" });
                return Ok(new { message = "Registration cancelled successfully" });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("user/{userId}/event/{eventId}")]
        public async Task<IActionResult> GetByUserAndEvent(int userId, int eventId)
        {
            var result = await _registrationService.GetByUserAndEventAsync(userId, eventId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetByEvent(int eventId)
        {
            var result = await _registrationService.GetByEventAsync(eventId);
            return Ok(result);
        }

        [HttpGet("my-registrations")]
        [Authorize]
        public async Task<IActionResult> GetMyRegistrations()
        {
            var userId = int.Parse(User.FindFirst("id")?.Value!);
            var regs = await _registrationService.GetRegistrationsByUserAsync(userId);
            return Ok(regs);
        }

    }
}
