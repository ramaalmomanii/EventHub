using EventHub.Core.DTOs.Payments;
using EventHub.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EventHub.Core.Exceptions;




namespace EventHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentCreateDto dto)
        {
            try
            {
                var result = await _paymentService.ProcessPaymentAsync(dto);
                return Ok(result);
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

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetByUser(int userId)
        {
            try
            {
                var payments = await _paymentService.GetByUserAsync(userId);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("event/{eventId}")]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> GetByEvent(int eventId)
        {
            try
            {
                var payments = await _paymentService.GetByEventAsync(eventId);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
