using AutoMapper;
using EventHub.Core.DTOs;
using EventHub.Core.DTOs.Events;
using EventHub.Core.Entities;
using EventHub.Core.Interfaces.Services;
using EventHub.Core.Repositories;
using EventHub.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Infrastructure.Services
{
    public class EventService : IEventService
    {
        private readonly IGenericRepository<EEvent> _repository;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Registration> _registrationRepository;



        public EventService(
       IRegistrationRepository registrationRepository,
       IEventRepository repository,
       IMapper mapper)
        {
            _registrationRepository = registrationRepository ?? throw new ArgumentNullException(nameof(registrationRepository));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
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

        public async Task<EventReadDto> UpdateAsync(int id, EventUpdateDto dto)
        {
            if (id <= 0)
                throw new ValidationException("Invalid event ID");

            if (dto == null)
                throw new ValidationException("Event data is required");

            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new NotFoundException($"Event with ID {id} not found");

            _mapper.Map(dto, existing);
            await _repository.UpdateAsync(existing);

            return _mapper.Map<EventReadDto>(existing);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Invalid event ID");

            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new NotFoundException($"Event with ID {id} not found");

            await _repository.DeleteAsync(id);
        }


     
        public async Task<IEnumerable<EventReadDto>> GetEventsByUserAsync(int userId)
        {
            var registrations = await _registrationRepository.FindAsync(r => r.AttendeeId == userId);

            if (registrations == null || !registrations.Any())
                return new List<EventReadDto>();


            var eventIds = registrations.Select(r => r.EventId).ToList();

            var events = await _repository.FindAsync(e => eventIds.Contains(e.Id));

            return _mapper.Map<IEnumerable<EventReadDto>>(events);
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

            // Update available seats
            ev.AvailableSeats--;
            await _repository.UpdateAsync(ev);
        }

        public async Task UnregisterUserFromEventAsync(int eventId, int userId)
        {
            if (eventId <= 0)
                throw new ValidationException("Invalid event ID");

            if (userId <= 0)
                throw new ValidationException("Invalid user ID");

            var registration = (await _registrationRepository.FindAsync(r => r.EventId == eventId && r.AttendeeId == userId)).FirstOrDefault();
            if (registration == null)
                throw new NotFoundException("Registration not found");

            await _registrationRepository.DeleteAsync(registration.Id);

            // Update available seats
            var ev = await _repository.GetByIdAsync(eventId);
            if (ev != null)
            {
                ev.AvailableSeats++;
                await _repository.UpdateAsync(ev);
            }
        }
    }
}
