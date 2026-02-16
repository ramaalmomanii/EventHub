using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventHub.Core.DTOs.Payments;
using EventHub.Core.Entities;
using EventHub.Core.Interfaces.Services;
using EventHub.Core.Repositories;
using iText.Layout.Element;


namespace EventHub.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly IRegistrationRepository _registrationRepo;
        private readonly IGenericRepository<Payment> _genericRepo;

        public PaymentService(IPaymentRepository paymentRepo, IRegistrationRepository registrationRepo)
        {
            _paymentRepo = paymentRepo;
            _registrationRepo = registrationRepo;
        }

        public async Task<PaymentReadDto> ProcessPaymentAsync(PaymentCreateDto dto)
        {
            var registration = await _registrationRepo.GetByIdAsync(dto.RegistrationId);
            if (registration == null)
                throw new Exception("Registration not found.");

            var payment = new Payment
            {
                RegistrationId = dto.RegistrationId,
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod,
                Status = "Paid", // integrate with real payment gateway
                PaidAt = DateTime.UtcNow
            };

            await _paymentRepo.AddAsync(payment);

            // Update registration status
            registration.Status = "Paid";
            await _registrationRepo.UpdateAsync(registration);

            return new PaymentReadDto
            {
                Id = payment.Id,
                EventTitle = registration.Event.Title,
                Amount = payment.Amount,
                Status = payment.Status,
                PaidAt = payment.PaidAt
            };
        }

        public async Task<IEnumerable<PaymentReadDto>> GetByUserAsync(int userId)
        {
            var payments = await _paymentRepo.GetByUserAsync(userId);
            return payments.Select(p => new PaymentReadDto
            {
                Id = p.Id,
                EventTitle = p.Registration.Event.Title,
                Amount = p.Amount,
                Status = p.Status,
                PaidAt = p.PaidAt
            });
        }

        public async Task<IEnumerable<PaymentReadDto>> GetByEventAsync(int eventId)
        {
            var payments = await _paymentRepo.GetByEventAsync(eventId);
            return payments.Select(p => new PaymentReadDto
            {
                Id = p.Id,
                EventTitle = p.Registration.Event.Title,
                Amount = p.Amount,
                Status = p.Status,
                PaidAt = p.PaidAt
            });
        }


        // ========  IGenericService methods ========
        public async Task AddAsync(PaymentReadDto dto)
        {
            var registration = await _registrationRepo.GetByIdAsync(dto.Id);
            if (registration == null)
                throw new Exception("Registration not found.");

            var payment = new Payment
            {
                RegistrationId = dto.Id,
                Amount = dto.Amount,
                PaymentMethod = "Unknown",
                Status = dto.Status,
                PaidAt = dto.PaidAt
            };
            await _paymentRepo.AddAsync(payment);
        }

        public async Task UpdateAsync(PaymentReadDto dto)
        {
            var payment = await _paymentRepo.GetByIdAsync(dto.Id);
            if (payment == null)
                throw new Exception("Payment not found.");

            payment.Amount = dto.Amount;
            payment.Status = dto.Status;
            payment.PaidAt = dto.PaidAt;

            await _paymentRepo.UpdateAsync(payment);
        }

        public async Task DeleteAsync(int id)
        {
            await _paymentRepo.DeleteAsync(id);
        }

        public async Task<List<PaymentReadDto>> GetAllAsync()
        {
            var payments = await _paymentRepo.GetAllAsync();
            return payments.Select(p => new PaymentReadDto
            {
                Id = p.Id,
                EventTitle = p.Registration.Event.Title,
                Amount = p.Amount,
                Status = p.Status,
                PaidAt = p.PaidAt
            }).ToList();
        }

        public async Task<PaymentReadDto> GetByIdAsync(int id)
        {
            var payment = await _paymentRepo.GetByIdAsync(id);
            if (payment == null) return null;

            return new PaymentReadDto
            {
                Id = payment.Id,
                EventTitle = payment.Registration.Event.Title,
                Amount = payment.Amount,
                Status = payment.Status,
                PaidAt = payment.PaidAt
            };
        }
    }
}