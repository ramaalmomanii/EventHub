using AutoMapper;
using EventHub.Core.Constants;
using EventHub.Core.DTOs;
using EventHub.Core.DTOs.Events;
using EventHub.Core.Entities;
using EventHub.Core.Exceptions;
using EventHub.Core.Interfaces.Services;
using EventHub.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Infrastructure.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _repository;
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IMapper _mapper;

        public EventService(
            IEventRepository repository,
            IRegistrationRepository registrationRepository,
            IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _registrationRepository = registrationRepository ?? throw new ArgumentNullException(nameof(registrationRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<EventReadDto>> GetAllAsync()
        {
            var events = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<EventReadDto>>(events);
        }

        public async Task<EventReadDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Invalid event ID");

            var ev = await _repository.GetByIdAsync(id);
            return ev == null ? null : _mapper.Map<EventReadDto>(ev);
        }

        public async Task<EventReadDto> AddAsync(EventCreateDto dto, int currentUserId)
        {
            if (dto == null)
                throw new ValidationException("Event data is required");

            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ValidationException("Event title is required");

            if (dto.StartDate < DateTime.UtcNow)
                throw new ValidationException("Start date cannot be in the past");

            if (dto.EndDate <= dto.StartDate)
                throw new ValidationException("End date must be after start date");

            if (dto.Capacity <= 0)
                throw new ValidationException("Capacity must be greater than zero");

            var ev = _mapper.Map<EEvent>(dto);
            ev.OrganizerId = currentUserId;
            ev.CreatedAt = DateTime.UtcNow;
            ev.AvailableSeats = dto.Capacity;
            ev.Status = "Pending";

            await _repository.AddAsync(ev);
            return _mapper.Map<EventReadDto>(ev);
        }

        public async Task<EventReadDto> UpdateAsync(int id, EventUpdateDto dto, int currentUserId, string role)
        {
            if (id <= 0)
                throw new ValidationException("Invalid event ID");

            if (dto == null)
                throw new ValidationException("Event data is required");

            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new NotFoundException($"Event with ID {id} not found");

            if (role == Permissions.Organizer && existing.OrganizerId != currentUserId)
                throw new ForbiddenException("You cannot update events created by other organizers");

            if (dto.StartDate < DateTime.UtcNow)
                throw new ValidationException("Start date cannot be in the past");

            if (dto.EndDate <= dto.StartDate)
                throw new ValidationException("End date must be after start date");

            if (dto.Capacity <= 0)
                throw new ValidationException("Capacity must be greater than zero");

            if (dto.Capacity != existing.Capacity)
            {
                var registeredCount = existing.Capacity - existing.AvailableSeats;
                if (dto.Capacity < registeredCount)
                    throw new ValidationException("New capacity cannot be less than the number of registered attendees");

                existing.AvailableSeats = dto.Capacity - registeredCount;
            }

            _mapper.Map(dto, existing);
            await _repository.UpdateAsync(existing);

            return _mapper.Map<EventReadDto>(existing);
        }

        public async Task DeleteAsync(int id, int currentUserId, string role)
        {
            if (id <= 0)
                throw new ValidationException("Invalid event ID");

            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new NotFoundException($"Event with ID {id} not found");

            if (role == Permissions.Organizer && existing.OrganizerId != currentUserId)
                throw new ForbiddenException("You cannot delete events created by other organizers");

            await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<EventReadDto>> GetEventsByUserAsync(int userId)
        {
            if (userId <= 0)
                throw new ValidationException("Invalid user ID");

            var events = await _repository.GetByOrganizerAsync(userId);
            return _mapper.Map<IEnumerable<EventReadDto>>(events);
        }

        public async Task<EventReadDto> UpdateStatusAsync(int id, string status, int currentUserId, string role)
        {
            if (id <= 0)
                throw new ValidationException("Invalid event ID");

            var validStatuses = new[] { "Pending", "Upcoming", "Ongoing", "Completed", "Cancelled" };
            if (!validStatuses.Contains(status))
                throw new ValidationException($"Invalid status. Valid values: {string.Join(", ", validStatuses)}");

            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new NotFoundException($"Event with ID {id} not found");

            if (role == Permissions.Organizer && existing.OrganizerId != currentUserId)
                throw new ForbiddenException("You cannot update status of events created by other organizers");

            existing.Status = status;
            await _repository.UpdateAsync(existing);

            return _mapper.Map<EventReadDto>(existing);
        }

        public async Task RegisterUserToEventAsync(int eventId, int userId)
        {
            if (eventId <= 0)
                throw new ValidationException("Invalid event ID");

            if (userId <= 0)
                throw new ValidationException("Invalid user ID");

            var ev = await _repository.GetByIdAsync(eventId);
            if (ev == null)
                throw new NotFoundException($"Event with ID {eventId} not found");

            if (ev.Status != "Upcoming")
                throw new ValidationException("Cannot register to an event that is not upcoming");

            if (ev.AvailableSeats <= 0)
                throw new ConflictException("Event is full");

            var existing = await _registrationRepository.FindAsync(r => r.EventId == eventId && r.AttendeeId == userId);
            if (existing.Any())
                throw new ConflictException("User is already registered for this event");

            var registration = new Registration
            {
                EventId = eventId,
                AttendeeId = userId,
                RegistrationDate = DateTime.UtcNow
            };

            await _registrationRepository.AddAsync(registration);

            ev.AvailableSeats--;
            await _repository.UpdateAsync(ev);
        }

        public async Task UnregisterUserFromEventAsync(int eventId, int userId)
        {
            if (eventId <= 0)
                throw new ValidationException("Invalid event ID");

            if (userId <= 0)
                throw new ValidationException("Invalid user ID");

            var registration = (await _registrationRepository.FindAsync(
                r => r.EventId == eventId && r.AttendeeId == userId)).FirstOrDefault();

            if (registration == null)
                throw new NotFoundException("Registration not found");

            await _registrationRepository.DeleteAsync(registration.Id);

            var ev = await _repository.GetByIdAsync(eventId);
            if (ev != null)
            {
                ev.AvailableSeats++;
                await _repository.UpdateAsync(ev);
            }
        }
    }
}
