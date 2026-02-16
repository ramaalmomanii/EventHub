using AutoMapper;
using EventHub.Core.DTOs.Registertions;
using EventHub.Core.DTOs.Ticket;
using EventHub.Core.Entities;
using EventHub.Core.Interfaces.Services;
using EventHub.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iText.Kernel.Pdf;

namespace EventHub.Infrastructure.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITicketService _ticketService; 

        public RegistrationService(
             IRegistrationRepository registrationRepository,
             IEventRepository eventRepository,
             IUserRepository userRepository,
             ITicketService ticketService) 
        {
            _registrationRepository = registrationRepository;
            _eventRepository = eventRepository;
            _userRepository = userRepository;
            _ticketService = ticketService;
        }

        public async Task<RegistrationReadDto> RegisterAsync(RegistrationCreateDto dto, int userId)
        {
            var ev = await _eventRepository.GetByIdAsync(dto.EventId);
            if (ev == null)
                throw new Exception("Event not found");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            var existing = await _registrationRepository.GetByUserAndEventAsync(userId, dto.EventId);

            if (existing != null)
            {
                if (existing.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
                {
                    if (ev.AvailableSeats <= 0)
                        throw new Exception("No available seats left for this event");

                    existing.Status = "Confirmed";
                    existing.RegistrationDate = DateTime.UtcNow;
                    ev.AvailableSeats -= 1;

                    await _registrationRepository.UpdateAsync(existing);
                    await _eventRepository.UpdateAsync(ev);

                    // Generate ticket for re-activated registration
                    await _ticketService.CreateAsync(new TicketCreateDto
                    {
                        RegistrationId = existing.Id,
                        //Attendee.FullName = FullName
                        UserId = userId,
                        EventId = dto.EventId,
                        SeatNumber = $"S-{existing.Id}",
                        Price = ev.Price
                    });

                    return MapToDto(existing, ev.Title);
                }

                throw new Exception("User already registered");
            }

            if (ev.AvailableSeats <= 0)
                throw new Exception("No available seats left for this event");

            var registration = new Registration
            {
                EventId = dto.EventId,
                AttendeeId = userId,
                RegistrationDate = DateTime.UtcNow,
                Status = "Confirmed"
            };

            ev.AvailableSeats -= 1;

            await _eventRepository.UpdateAsync(ev);
            await _registrationRepository.AddAsync(registration);

            // Generate ticket for new registration
            await _ticketService.CreateAsync(new TicketCreateDto
            {
                RegistrationId = registration.Id,
                UserId = userId,
                EventId = dto.EventId,
                SeatNumber = $"S-{registration.Id}",
                Price = ev.Price
            });

            return MapToDto(registration, ev.Title);
        }

        private RegistrationReadDto MapToDto(Registration reg, string eventTitle)
        {
            return new RegistrationReadDto
            {
                Id = reg.Id,
                EventId = reg.EventId,
                AttendeeId = reg.AttendeeId,
                EventTitle = eventTitle,
                RegistrationDate = reg.RegistrationDate,
                Status = reg.Status
            };
        }
    
        public async Task<List<RegistrationReadDto>> GetByEventAsync(int eventId)
        {
            var registrations = await _registrationRepository.GetByEventAsync(eventId);
            if (registrations == null || !registrations.Any())
                return new List<RegistrationReadDto>();
            var ev = await _eventRepository.GetByIdAsync(eventId);
            var eventTitle = ev != null ? ev.Title : "Unknown";
            return registrations.Select(r => new RegistrationReadDto
            {
                Id = r.Id,
                EventId = r.EventId,
                AttendeeId = r.AttendeeId,
                EventTitle = eventTitle,
                RegistrationDate = r.RegistrationDate,
                Status = r.Status
            }).ToList();
        }

        public async Task<IEnumerable<RegistrationReadDto>> GetRegistrationsByUserAsync(int userId)
        {
            var registrations = await _registrationRepository.FindAsync(r => r.AttendeeId == userId);

            if (registrations == null || !registrations.Any())
                return new List<RegistrationReadDto>();

            var events = await _eventRepository.GetAllAsync();

            return registrations.Select(r => new RegistrationReadDto
            {
                Id = r.Id,
                EventId = r.EventId,
                AttendeeId = r.AttendeeId,
                EventTitle = events.FirstOrDefault(e => e.Id == r.EventId)?.Title ?? "Unknown",
                RegistrationDate = r.RegistrationDate,
                Status = r.Status
            }).ToList();
        }

        public async Task<RegistrationReadDto?> GetByUserAndEventAsync(int userId, int eventId)
        {
            var reg = await _registrationRepository.GetByUserAndEventAsync(userId, eventId);
            if (reg == null) return null;
            var ev = await _eventRepository.GetByIdAsync(eventId);
            var eventTitle = ev != null ? ev.Title : "Unknown";
            return new RegistrationReadDto
            {
                Id = reg.Id,
                EventId = reg.EventId,
                AttendeeId = reg.AttendeeId,
                EventTitle = eventTitle,
                RegistrationDate = reg.RegistrationDate,
                Status = reg.Status
            };
        }



        public async Task<bool> CancelRegistrationAsync(int registrationId)
        {
            var reg = await _registrationRepository.GetByIdAsync(registrationId);
            if (reg == null) return false;
            if (reg.Status == "Cancelled") return false;

            reg.Status = "Cancelled";
            await _registrationRepository.UpdateAsync(reg);

            var ev = await _eventRepository.GetByIdAsync(reg.EventId);
            if (ev != null)
            {
                ev.AvailableSeats += 1;
                await _eventRepository.UpdateAsync(ev);
            }

            return true;
        }
    }
}
