using EventHub.Core.DTOs;
using EventHub.Core.DTOs.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Core.Interfaces.Services
{
    public interface IPaymentService : IGenericService<PaymentReadDto>
    {
        Task<PaymentReadDto> ProcessPaymentAsync(PaymentCreateDto dto);
        Task<IEnumerable<PaymentReadDto>> GetByUserAsync(int userId);
        Task<IEnumerable<PaymentReadDto>> GetByEventAsync(int eventId);
    }
}

