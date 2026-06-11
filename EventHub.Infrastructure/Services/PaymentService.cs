using EventHub.Core.Constants;
using EventHub.Core.DTOs.Payments;
using EventHub.Core.Entities;
using EventHub.Core.Exceptions;
using EventHub.Core.Interfaces.Services;
using EventHub.Core.Repositories;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EventHub.Infrastructure.Services
{
        public class PaymentService : IPaymentService
        {
            private readonly IPaymentRepository _paymentRepo;
            private readonly IRegistrationRepository _registrationRepo;

            public PaymentService(IPaymentRepository paymentRepo, IRegistrationRepository registrationRepo)
            {
                _paymentRepo = paymentRepo;
                _registrationRepo = registrationRepo;
            }

            public async Task<PaymentReadDto> ProcessPaymentAsync(PaymentCreateDto dto, int currentUserId)
            {
                if (dto == null)
                    throw new ValidationException("Payment data is required");

                if (dto.Amount <= 0)
                    throw new ValidationException("Amount must be greater than zero");

                if (string.IsNullOrWhiteSpace(dto.PaymentMethod))
                    throw new ValidationException("Payment method is required");

                var registration = await _registrationRepo.GetByIdWithEventAsync(dto.RegistrationId);
                if (registration == null)
                    throw new NotFoundException("Registration not found");

                if (registration.AttendeeId != currentUserId)
                    throw new ForbiddenException("You cannot pay for another user's registration");

                if (registration.Status == "Paid")
                    throw new ConflictException("This registration is already paid");

                var payment = new Payment
                {
                    RegistrationId = dto.RegistrationId,
                    Amount = dto.Amount,
                    PaymentMethod = dto.PaymentMethod,
                    Status = "Paid",
                    PaidAt = DateTime.UtcNow
                };

                await _paymentRepo.AddAsync(payment);

                registration.Status = "Paid";
                await _registrationRepo.UpdateAsync(registration);

                return new PaymentReadDto
                {
                    Id = payment.Id,
                    RegistrationId = payment.RegistrationId,
                    EventTitle = registration.Event.Title,
                    PaymentMethod = payment.PaymentMethod,
                    Amount = payment.Amount,
                    Status = payment.Status,
                    PaidAt = payment.PaidAt
                };
            }

            public async Task<IEnumerable<PaymentReadDto>> GetByUserAsync(int userId)
            {
                if (userId <= 0)
                    throw new ValidationException("Invalid user ID");

                var payments = await _paymentRepo.GetByUserAsync(userId);
                return payments.Select(p => new PaymentReadDto
                {
                    Id = p.Id,
                    RegistrationId = p.RegistrationId,
                    EventTitle = p.Registration.Event.Title,
                    PaymentMethod = p.PaymentMethod,
                    Amount = p.Amount,
                    Status = p.Status,
                    PaidAt = p.PaidAt
                });
            }

            public async Task<IEnumerable<PaymentReadDto>> GetByEventAsync(int eventId, int currentUserId, string role)
            {
                if (eventId <= 0)
                    throw new ValidationException("Invalid event ID");

                var payments = await _paymentRepo.GetByEventAsync(eventId);

                if (role == Permissions.Organizer)
                {
                    payments = payments.Where(p => p.Registration.Event.OrganizerId == currentUserId);
                }

                return payments.Select(p => new PaymentReadDto
                {
                    Id = p.Id,
                    RegistrationId = p.RegistrationId,
                    EventTitle = p.Registration.Event.Title,
                    PaymentMethod = p.PaymentMethod,
                    Amount = p.Amount,
                    Status = p.Status,
                    PaidAt = p.PaidAt
                });
            }

            public async Task<PaymentReadDto> GetByIdAsync(int id)
            {
                if (id <= 0)
                    throw new ValidationException("Invalid payment ID");

                var payment = await _paymentRepo.GetByIdAsync(id);
                if (payment == null)
                    throw new NotFoundException($"Payment with ID {id} not found");

                return new PaymentReadDto
                {
                    Id = payment.Id,
                    RegistrationId = payment.RegistrationId,
                    EventTitle = payment.Registration.Event.Title,
                    PaymentMethod = payment.PaymentMethod,
                    Amount = payment.Amount,
                    Status = payment.Status,
                    PaidAt = payment.PaidAt
                };
            }

            public async Task<IEnumerable<PaymentReadDto>> GetAllAsync()
            {
                var payments = await _paymentRepo.GetAllAsync();
                return payments.Select(p => new PaymentReadDto
                {
                    Id = p.Id,
                    RegistrationId = p.RegistrationId,
                    EventTitle = p.Registration.Event.Title,
                    PaymentMethod = p.PaymentMethod,
                    Amount = p.Amount,
                    Status = p.Status,
                    PaidAt = p.PaidAt
                });
            }
        }
    
}