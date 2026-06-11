using EventHub.Core.DTOs;
using EventHub.Core.DTOs.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Core.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<PaymentReadDto> ProcessPaymentAsync(PaymentCreateDto dto, int currentUserId);
        Task<IEnumerable<PaymentReadDto>> GetByUserAsync(int userId);
        Task<IEnumerable<PaymentReadDto>> GetByEventAsync(int eventId, int currentUserId, string role);
        Task<PaymentReadDto> GetByIdAsync(int id);
        Task<IEnumerable<PaymentReadDto>> GetAllAsync();
    }
}
