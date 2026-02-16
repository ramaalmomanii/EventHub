using EventHub.Core.DTOs;
using EventHub.Core.DTOs.Events;
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
    public class EventController : ControllerBase
    {
        private readonly IEventService _service;

        public EventController(IEventService service)
        {
            _service = service;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventReadDto>>> GetAll()
        {
            try
            {
                return Ok(await _service.GetAllAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // get event by id
        [HttpGet("{id}")]
        public async Task<ActionResult<EventReadDto>> GetById(int id)
        {
            try
            {
                var ev = await _service.GetByIdAsync(id);
                if (ev == null) 
                    return NotFound(new { message = "Event not found" });
                return Ok(ev);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,Organizer")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EventCreateDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst("id")?.Value;
                if (userIdClaim == null) 
                    return Unauthorized(new { message = "Invalid token" });

                var userId = int.Parse(userIdClaim);
                dto.OrganizerId = userId;

                var createdEvent = await _service.AddAsync(dto, userId);
                return CreatedAtAction(nameof(GetById), new { id = createdEvent.Id }, createdEvent);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,Organizer")]
        [HttpPut("{id}")]
        public async Task<ActionResult<EventReadDto>> Update(int id, [FromBody] EventUpdateDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst("id")?.Value;
                if (userIdClaim == null) 
                    return Unauthorized(new { message = "Invalid token" });

                var userId = int.Parse(userIdClaim);
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                var ev = await _service.GetByIdAsync(id);
                if (ev == null) 
                    return NotFound(new { message = "Event not found" });

                // Only admin or organizer who created the event can update it
                if (role == "Organizer" && ev.OrganizerId != userId)
                    return Forbid(new { message = "You cannot update events created by other organizers." });

                var updated = await _service.UpdateAsync(id, dto);
                return Ok(updated);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,Organizer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst("id")?.Value;
                if (userIdClaim == null) 
                    return Unauthorized(new { message = "Invalid token" });

                var userId = int.Parse(userIdClaim);
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                var ev = await _service.GetByIdAsync(id);
                if (ev == null) 
                    return NotFound(new { message = "Event not found" });

                // Only admin or organizer who created the event can delete it
                if (role == "Organizer" && ev.OrganizerId != userId)
                    return Forbid(new { message = "You cannot delete events created by other organizers." });

                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("my-events")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<EventReadDto>>> GetMyEvents()
        {
            try
            {
                var userIdClaim = User.FindFirst("id")?.Value;
                if (userIdClaim == null) 
                    return Unauthorized(new { message = "Invalid token" });

                var userId = int.Parse(userIdClaim);
                var events = await _service.GetEventsByUserAsync(userId);
                return Ok(events);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
       

       


    }
}
