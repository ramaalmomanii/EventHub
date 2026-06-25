using AutoMapper;
using EventHub.Core.Constants;
using EventHub.Core.DTOs.Registertions;
using EventHub.Core.DTOs.Ticket;
using EventHub.Core.Entities;
using EventHub.Core.Exceptions;
using EventHub.Core.Interfaces.Services;
using EventHub.Core.Repositories;
using iText.Kernel.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Infrastructure.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ITicketService _ticketService;

        public RegistrationService(
            IRegistrationRepository registrationRepository,
            IEventRepository eventRepository,
            ITicketService ticketService)
        {
            _registrationRepository = registrationRepository;
            _eventRepository = eventRepository;
            _ticketService = ticketService;
        }

        public async Task<RegistrationReadDto> RegisterAsync(RegistrationCreateDto dto, int userId)
        {
            if (dto == null)
                throw new ValidationException("Registration data is required");

            var ev = await _eventRepository.GetByIdAsync(dto.EventId);
            if (ev == null)
                throw new NotFoundException("Event not found");

            await SyncEventStatusAsync(ev);

            if (ev.Status != "Upcoming")
                throw new ValidationException("Cannot register to an event that is not upcoming");

            if (ev.AvailableSeats <= 0)
                throw new ConflictException("No available seats left for this event");

            var existing = await _registrationRepository.GetByUserAndEventAsync(userId, dto.EventId);

            if (existing != null)
            {
                if (existing.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
                {
                    existing.Status = "Confirmed";
                    existing.RegistrationDate = DateTime.UtcNow;
                    ev.AvailableSeats--;

                    await _registrationRepository.UpdateAsync(existing);
                    await _eventRepository.UpdateAsync(ev);

                    await _ticketService.CreateAsync(new TicketCreateDto
                    {
                        RegistrationId = existing.Id,
                        UserId = userId,
                        EventId = dto.EventId,
                        SeatNumber = $"S-{existing.Id}",
                        Price = ev.Price
                    });

                    return MapToDto(existing, ev);
                }

                throw new ConflictException("User already registered for this event");
            }

            var registration = new Registration
            {
                EventId = dto.EventId,
                AttendeeId = userId,
                RegistrationDate = DateTime.UtcNow,
                Status = "Confirmed"
            };

            ev.AvailableSeats--;
            await _eventRepository.UpdateAsync(ev);
            await _registrationRepository.AddAsync(registration);

            await _ticketService.CreateAsync(new TicketCreateDto
            {
                RegistrationId = registration.Id,
                UserId = userId,
                EventId = dto.EventId,
                SeatNumber = $"S-{registration.Id}",
                Price = ev.Price
            });

            return MapToDto(registration, ev);
        }

        public async Task CancelRegistrationAsync(int registrationId, int currentUserId, string role)
        {
            var reg = await _registrationRepository.GetByIdAsync(registrationId);
            if (reg == null)
                throw new NotFoundException("Registration not found");

            if (role == Permissions.Attendee && reg.AttendeeId != currentUserId)
                throw new ForbiddenException("You cannot cancel another user's registration");

            if (reg.Status == "Cancelled")
                throw new ConflictException("Registration is already cancelled");

            reg.Status = "Cancelled";
            await _registrationRepository.UpdateAsync(reg);

            var ev = await _eventRepository.GetByIdAsync(reg.EventId);
            if (ev != null)
            {
                ev.AvailableSeats++;
                await _eventRepository.UpdateAsync(ev);
            }
        }

        public async Task<RegistrationReadDto?> GetByUserAndEventAsync(int userId, int eventId)
        {
            var reg = await _registrationRepository.GetByUserAndEventAsync(userId, eventId);
            if (reg == null) return null;

            var ev = await _eventRepository.GetByIdAsync(eventId);
            if (ev == null)
                return null;

            await SyncEventStatusAsync(ev);
            return MapToDto(reg, ev);
        }

        public async Task<IEnumerable<RegistrationReadDto>> GetByEventAsync(int eventId)
        {
            if (eventId <= 0)
                throw new ValidationException("Invalid event ID");

            var registrations = await _registrationRepository.GetByEventAsync(eventId);
            var ev = await _eventRepository.GetByIdAsync(eventId);
            if (ev == null)
                throw new NotFoundException("Event not found");

            await SyncEventStatusAsync(ev);

            return registrations.Select(r => MapToDto(r, ev));
        }

        public async Task<IEnumerable<RegistrationReadDto>> GetRegistrationsByUserAsync(int userId)
        {
            if (userId <= 0)
                throw new ValidationException("Invalid user ID");

            // GetByUserAsync 
            var registrations = await _registrationRepository.GetByUserAsync(userId);
            foreach (var registration in registrations)
            {
                if (registration.Event != null)
                    await SyncEventStatusAsync(registration.Event);
            }

            return registrations.Select(r => new RegistrationReadDto
            {
                Id = r.Id,
                EventId = r.EventId,
                EventTitle = r.Event?.Title ?? "Unknown",
                AttendeeId = r.AttendeeId,
                AttendeeName = r.Attendee?.FullName ?? "Unknown",
                RegistrationDate = r.RegistrationDate,
                Status = r.Status,
                EventEndDate = r.Event?.EndDate ?? DateTime.MinValue,
                EventStatus = r.Event?.Status ?? "Unknown"
            });
        }

        // ====== Helper ======
        private static RegistrationReadDto MapToDto(Registration reg, EEvent ev)
        {
            return new RegistrationReadDto
            {
                Id = reg.Id,
                EventId = reg.EventId,
                EventTitle = ev.Title,
                AttendeeId = reg.AttendeeId,
                AttendeeName = reg.Attendee?.FullName ?? "Unknown",
                RegistrationDate = reg.RegistrationDate,
                Status = reg.Status,
                EventEndDate = ev.EndDate,
                EventStatus = ev.Status
            };
        }
        private async Task SyncEventStatusAsync(EEvent ev)
        {
            var previousStatus = ev.Status;
            SetStatusFromSchedule(ev);

            if (ev.Status != previousStatus)
            {
                await _eventRepository.UpdateAsync(ev);
            }
        }

        private static void SetStatusFromSchedule(EEvent ev)
        {
            if (ev.Status == "Cancelled")
                return;

            var now = DateTime.UtcNow;
            if (ev.EndDate <= now)
            {
                ev.Status = "Inactive";
            }
            else if (ev.StartDate <= now)
            {
                ev.Status = "Active";
            }
            else
            {
                ev.Status = "Upcoming";
            }
        }
    }
}
