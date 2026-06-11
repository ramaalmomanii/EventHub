using EventHub.Core.Constants;
using EventHub.Core.DTOs;
using EventHub.Core.DTOs.Ticket;
using EventHub.Core.Exceptions;
using EventHub.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [Authorize(Roles = $"{Permissions.Admin}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketReadDto>>> GetAll()
        {
            return Ok(await _ticketService.GetAllAsync());
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<TicketReadDto>> GetById(int id)
        {
            var ticket = await _ticketService.GetByIdAsync(id);
            if (ticket == null)
                return NotFound(new { message = "Ticket not found" });
            return Ok(ticket);
        }

        [Authorize]
        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<TicketReadDto>>> GetMyTickets()
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            return Ok(await _ticketService.GetByUserIdAsync(userId));
        }

        [Authorize(Roles = $"{Permissions.Admin}")]
        [HttpPut("{id}")]
        public async Task<ActionResult<TicketReadDto>> Update(int id, [FromBody] TicketUpdateDto dto)
        {
            var updated = await _ticketService.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [Authorize(Roles = $"{Permissions.Admin}")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _ticketService.DeleteAsync(id);
            return NoContent();
        }

        [Authorize]
        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadPdf(int id)
        {
            var ticket = await _ticketService.GetByIdAsync(id);
            if (ticket == null || string.IsNullOrEmpty(ticket.PdfPath))
                return NotFound(new { message = "Ticket or PDF not found" });

            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot",
                ticket.PdfPath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
            );

            if (!System.IO.File.Exists(filePath))
                return NotFound(new { message = "PDF file not found" });

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/pdf", $"Ticket_{ticket.Id}.pdf");
        }
    }
}
