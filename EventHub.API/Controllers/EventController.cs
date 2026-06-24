using EventHub.Core.Constants;
using EventHub.Core.DTOs;
using EventHub.Core.DTOs.Events;
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
    public class EventController : ControllerBase
    {
        private readonly IEventService _service;
        private readonly IEventSummaryService _summaryService;

        public EventController(IEventService service, IEventSummaryService summaryService)
        {
            _service = service;
            _summaryService = summaryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventReadDto>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventReadDto>> GetById(int id)
        {
            var ev = await _service.GetByIdAsync(id);
            if (ev == null)
                return NotFound(new { message = "Event not found" });
            return Ok(ev);
        }

        [Authorize(Roles = $"{Permissions.Admin},{Permissions.Organizer},{Permissions.Attendee}")]
        [HttpGet("{id}/summary")]
        public async Task<ActionResult<EventSummaryDto>> GetSummary(int id, [FromQuery] string provider = "openai")
        {
            var summary = await _summaryService.GetSummaryAsync(id, provider);
            return Ok(summary);
        }

        
        [HttpPost]
        [Authorize(Roles = $"{Permissions.Admin},{Permissions.Organizer}")]
        public async Task<ActionResult<EventReadDto>> Create([FromBody] EventCreateDto dto)
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            var created = await _service.AddAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{Permissions.Admin},{Permissions.Organizer}")]
        public async Task<ActionResult<EventReadDto>> Update(int id, [FromBody] EventUpdateDto dto)
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;
            var updated = await _service.UpdateAsync(id, dto, userId, role);
            return Ok(updated);
        }

        
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{Permissions.Admin},{Permissions.Organizer}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;
            await _service.DeleteAsync(id, userId, role);
            return NoContent();
        }

        [Authorize]
        [HttpGet("my-events")]
        public async Task<ActionResult<IEnumerable<EventReadDto>>> GetMyEvents()
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            return Ok(await _service.GetEventsByUserAsync(userId));
        }

        
        [HttpPatch("{id}/status")]
        [Authorize(Roles = $"{Permissions.Admin},{Permissions.Organizer}")]
        public async Task<ActionResult<EventReadDto>> UpdateStatus(int id, [FromBody] string status)
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;
            var updated = await _service.UpdateStatusAsync(id, status, userId, role);
            return Ok(updated);
        }
    }
}
