using EventHub.Core.DTOs.Ticket;
using EventHub.Core.DTOs;
using EventHub.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EventHub.Core.Exceptions;

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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var tickets = await _ticketService.GetAllAsync();
            return Ok(tickets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var ticket = await _ticketService.GetByIdAsync(id);
                if (ticket == null) 
                    return NotFound(new { message = "Ticket not found" });
                return Ok(ticket);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> GetMyTickets()
        {
            try
            {
                var userIdClaim = User.FindFirst("id")?.Value;
                if (userIdClaim == null) 
                    return Unauthorized(new { message = "Invalid token" });

                var userId = int.Parse(userIdClaim);
                var tickets = await _ticketService.GetByUserIdAsync(userId);
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] TicketCreateDto dto)
        {
            try
            {
                var ticket = await _ticketService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
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


        [HttpGet("{id}/download")]
        [Authorize]
        public async Task<IActionResult> DownloadPdf(int id)
        {
            var ticket = await _ticketService.GetByIdAsync(id);
            if (ticket == null || string.IsNullOrEmpty(ticket.PdfPath)) return NotFound();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", ticket.PdfPath);
            if (!System.IO.File.Exists(filePath)) return NotFound();

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/pdf", $"Ticket_{ticket.Id}.pdf");
        }


        [HttpGet("download/{fileName}")]
        public IActionResult DownloadTicket(string fileName)
        {
            var filePath = Path.Combine("wwwroot", "tickets", fileName);
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var mimeType = "application/pdf";
            return PhysicalFile(filePath, mimeType, fileName);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] TicketUpdateDto dto)
        {
            try
            {
                if (id != dto.Id) 
                    return BadRequest(new { message = "ID mismatch" });

                var updated = await _ticketService.UpdateAsync(dto);
                if (updated == null) 
                    return NotFound(new { message = "Ticket not found" });

                return Ok(updated);
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _ticketService.DeleteAsync(id);
                if (!success) 
                    return NotFound(new { message = "Ticket not found" });

                return NoContent();
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
    }
}
