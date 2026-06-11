using EventHub.Core.Constants;
using EventHub.Core.DTOs.Payments;
using EventHub.Core.Exceptions;
using EventHub.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


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

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<PaymentReadDto>> ProcessPayment([FromBody] PaymentCreateDto dto)
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            var result = await _paymentService.ProcessPaymentAsync(dto, userId);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("my-payments")]
        public async Task<ActionResult<IEnumerable<PaymentReadDto>>> GetMyPayments()
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            return Ok(await _paymentService.GetByUserAsync(userId));
        }

        [Authorize(Roles = $"{Permissions.Admin}")]
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<PaymentReadDto>>> GetByUser(int userId)
        {
            return Ok(await _paymentService.GetByUserAsync(userId));
        }

        [Authorize(Roles = $"{Permissions.Admin},{Permissions.Organizer}")]
        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<IEnumerable<PaymentReadDto>>> GetByEvent(int eventId)
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;
            return Ok(await _paymentService.GetByEventAsync(eventId, userId, role));
        }

        [Authorize(Roles = $"{Permissions.Admin}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentReadDto>>> GetAll()
        {
            return Ok(await _paymentService.GetAllAsync());
        }
    }
}
